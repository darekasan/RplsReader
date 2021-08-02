# RplsReader

BDAVのrplsファイルをパースする

### なにこれ

##### RplsReader

rplsファイルをパースするためのクラス群。作成や編集はできません。

番組名や番組説明のデコードに[hunamizawa/AribB24.DotNet](https://github.com/hunamizawa/AribB24.DotNet)を使用します。

##### RplsReader-cli

rplsファイルから番組名と番組詳細、マーカー情報を取得してjsonで出力します。

### 使い方(暫定)

    RplsReader-cli.exe <path to rpls file>

するとこんな感じで出力される

```
{
  "Date": "2018-01-05T01:30:00",
  "ChannelName": "ＢＳ１１イレブン",
  "Title": "ゆるキャン△\uD83C\uDE1F第１話「ふじさんとカレーメン」",
  "Detail": "一人キャンプが趣味のリンは、キャンプ場のベンチで眠りこけていた、なでしこと出会う。富士山が見える本栖湖の寒 空の下、なぜか二人は一緒に夜を過ごすことになり……。番組内容\r\n△新・日常系ガールズストーリー開幕。\r\n彼女たちが過ごす時間は、ちょっぴり手が届きそうな非日常。\r\n山梨を舞台に、女子高校生たちがキャンプをしたり、日常生活を送ったり…。\r\nこの冬、一緒にキャンプにいこう。\r\n\r\n出演者\r\n（各務原なでしこ）花守ゆみり\r\n（志摩リン）東山奈央\r\n（大垣千明）原紗友里\r\n（犬山あおい）豊崎愛生\r\n（斉藤恵那）高橋李依\r\n（各務原さくら）井上麻里奈\r\n（ナレーション）大塚明夫\r\nほか\r\n\r\nスタッフ\r\n【原作】あfろ（まんがタイムきららフォワード／芳文社刊）\r\n【監督】京極義昭\r\n【シリーズ構成】田中 仁\r\n【キャラクターデザイン】佐々木睦美\r\n【音楽】立山秋航\r\n【アニメーション制作】C-Station\r\n\r\n音楽\r\n【オープ ニングテーマ】\r\n『SHINY DAYS』／亜咲花\r\n【エンディングテーマ】\r\n『ふゆびより』／佐々木恵梨\r\n\r\nおしらせ\r\nBS11公式WEBサイトでは、みなさまからのメッセージを受け付け、公開しております。番組への率直なご意見やご感想など、どしどしお寄 せ下さい。お待ちしております！\r\nhttp://www.bs11.jp/anime/yurucamp/",
  "Playlist": {
    "Items": [
      {
        "Filename": "00001.M2TS",
        "In": 37618,
        "Out": 81462461
      }
    ]
  },
  "Marker": {
    "Items": [
      {
        "Valid": true,
        "PlaylistItemId": 0,
        "TimeText": "00:00:00",
        "TimeMillisecond": 0
      },
      {
        "Valid": true,
        "PlaylistItemId": 0,
        "TimeText": "00:00:06.5730000",
        "TimeMillisecond": 6573
      },
      {
        "Valid": false,
        "PlaylistItemId": 0,
        "TimeText": "00:00:10",
        "TimeMillisecond": 10000
      },
      {
        "Valid": true,
        "PlaylistItemId": 0,
        "TimeText": "00:02:48.5680000",
        "TimeMillisecond": 168568
      },
      {
        "Valid": true,
        "PlaylistItemId": 0,
        "TimeText": "00:03:48.5780000",
        "TimeMillisecond": 228578
      },
      {
        "Valid": true,
        "PlaylistItemId": 0,
        "TimeText": "00:09:53.0420000",
        "TimeMillisecond": 593042
      },
      {
        "Valid": true,
        "PlaylistItemId": 0,
        "TimeText": "00:14:22.5780000",
        "TimeMillisecond": 862578
      },
      {
        "Valid": true,
        "PlaylistItemId": 0,
        "TimeText": "00:15:22.5710000",
        "TimeMillisecond": 922571
      },
      {
        "Valid": true,
        "PlaylistItemId": 0,
        "TimeText": "00:21:05.0800000",
        "TimeMillisecond": 1265080
      },
      {
        "Valid": true,
        "PlaylistItemId": 0,
        "TimeText": "00:24:55.5770000",
        "TimeMillisecond": 1495577
      },
      {
        "Valid": true,
        "PlaylistItemId": 0,
        "TimeText": "00:25:55.5700000",
        "TimeMillisecond": 1555570
      },
      {
        "Valid": true,
        "PlaylistItemId": 0,
        "TimeText": "00:27:06.5740000",
        "TimeMillisecond": 1626574
      }
    ]
  }
}
```

### おまけ

使用例としてPowerShellスクリプト(convert_to_mp4.ps1)を書いてみました。decryptedなBDAVのフォルダを指定するとrplsを見つけて、いい感じのファイル名のチャプター付きmp4にしてくれます。

本当はRplsReader.dllを読み込んであれこれしたかったんですが、うまくいかなかったのでRplsReader-cli.exeが吐き出したjsonを使っています。

レコーダーでカット編集などを行ったものだと正しく動作しない可能性が高いです。

### 条件

ソニーのBDZ-EW1000で録画したものでしか動作確認していません。録画モードの違いや編集の有無によって動作しないかもしれません。