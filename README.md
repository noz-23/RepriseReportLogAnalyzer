# Reprise Report Log Analyzer

Reprise Licence Manager の Report Log を解析

## 1.概要

Reprise Licence Manager の Report Log を解析するアプリです。

複数のファイルは一つのファイルとして処理します。

順序は表示ファイルの順になりますので、日付が若い方を上にして下さい。

チェックアウトとチェックインの結合は一対ですが、同じユーザーに重複して出力する場合があります。

重複を考慮した結果を出力することができます。

出力は CSV と SQLite となります。


## 2.注意点

複数のサーバのに対しては考慮していません。

サーバ毎に解析処理をお願いします。

## 3.今後

もう少しソースと処理を綺麗に改善する予定です。

## 4.ライセンス

MIT license https://mit-license.org/

## 5.利用

Costura.Fody : MIT

 https://github.com/Fody/Costura

ScottPlot : MIT

 https://scottplot.net/

Dapper : Apache-2.0

 https://github.com/DapperLib/Dapper

SQLite : Public Domain

 https://system.data.sqlite.org/src/doc/trunk/www/index.wiki


Reprise Reportlog File Format

 https://reprisesoftware.com/docs/admin/reportlog-format.html

 ## 6.履歴
 0.1.0  : 2025/03/27 リリース　開始
 0.1.1  : 2025/03/27 リリース　不要なライブラリ削除 /コードの保守容易性指数の改善/ バグ修正