# convert_to_mp4.ps1
# decryptedなBDAVがあるフォルダ(深くてもOK)を指定するとrplsを見つけて
# 全部いい感じのファイル名でチャプター付きmp4にして出力してくれるやつ

$rrcli = "RplsReader-cli\bin\Release\RplsReader-cli.exe"
$ffmpeg = "ffmpeg.exe"

# rplsファイルを再帰的に探す
$rpls_files = Get-ChildItem -Recurse -File -Include *.rpls $args[0]

# 引数ないとき
if($args.Count -le 0){
    echo "usage : .\convert_to_mp4.ps1 <input dir> [output dir]"
}

if($args.Count -ge 2){
    # 第2引数あればそれを出力先フォルダとする
    $out_dir = $args[1]
}else{
    # 第2引数ないならカレントディレクトリを出力先とする
    $out_dir = Get-Location | Convert-Path;
}

$metadata_tmp = Join-Path $out_dir "ffmeta.txt"
function FFmeta([string]$key, [string]$value){
    $key + "=" + $value.Replace("`r`n","`n").Replace("=","\=").Replace(";","\;").Replace("#","\#").Replace("\","\\").Replace("`n","\`n") + "`n"
}

foreach($item in $rpls_files){
    # RplsReader-cli.exeでrplsをjsonにしてもらって取得
    $data = &$rrcli $item.FullName | ConvertFrom-Json

    $stream_path = Join-Path $item.FullName "../../STREAM" | Convert-Path

    # m2tsのパスを取得する
    $src_path = Join-Path $stream_path $data.Playlist.Items[0].Filename
    
    # 出力ファイル名を決めるとこ
    # $dst_path = Join-Path $out_dir ((([DateTime]$data.Date).ToString("yyyyMMdd"))+"-"+$data.Title+".mp4")
    $dst_path = Join-Path $out_dir ($data.Title+".mp4")

    # メタデータいろいろ
    $ffmeta = ";FFMETADATA1`n"
    $ffmeta += (FFmeta "creation_time" $data.Date)
    $ffmeta += (FFmeta "title" $data.Title)
    $ffmeta += (FFmeta "comment" $data.Detail)

    # RplsMarkerItemをffmpegに渡す形式にする
    $markers = $data.Marker.Items
    for ($i = 0; $i -lt $markers.Count; $i++) {

        if(!$markers[$i].Valid){
            continue
        }
        $in = $markers[$i].TimeMillisecond
        $out = 0

        if($i -eq $markers.Count-1){
            $durr = 0
            foreach($p in $data.Playlist.Items){
                $durr += $p.DurationMillisecond
            }
            $out = $durr
        }else{
            for ($j = $i+1; $j -lt $markers.Count; $j++) {
                if($markers[$j].Valid){
                    $out = $markers[$j].TimeMillisecond
                    break
                }
            }
        }

        $ffmeta += "[CHAPTER]" + "`n"
        $ffmeta += (FFmeta "TIMEBASE" "1/1000")
        
        $ffmeta += (FFmeta "START" $in)
        $ffmeta += (FFmeta "END" $out)

        $Utf8NoBomEncoding = New-Object System.Text.UTF8Encoding $False
        [System.IO.File]::WriteAllLines($metadata_tmp, $ffmeta, $Utf8NoBomEncoding)

    }

    echo $src_path
    echo $dst_path

    # 変換実行
    &$ffmpeg -i $src_path -i $metadata_tmp -map_metadata 1 -codec copy $dst_path 

    Remove-Item -Path $metadata_tmp
}
