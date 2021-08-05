# find_rpls.ps1
# BDAVなフォルダがたくさんあってどれがどれかわからない！ってときにつかう
# 1番目のrplsを再帰的に探してきてパスと番組名を表示するやつ

$rrcli = "RplsReader-cli\bin\Release\RplsReader-cli.exe"

# rplsファイルを再帰的に探す
$rpls_files = Get-ChildItem -Recurse -File -Include 00001.rpls $args[0]

# 引数ないとき
if($args.Count -le 0){
    echo "usage : .\find_rpls.ps1 <input dir>"
}

foreach($item in $rpls_files){
    # RplsReader-cli.exeでrplsをjsonにしてもらって取得
    $data = &$rrcli $item.FullName | ConvertFrom-Json

    $stream_path = Join-Path $item.FullName "../../STREAM" | Convert-Path

    $item.FullName
    $data.Title

    echo ""
}
