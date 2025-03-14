# Reprise Report Log Analyzer

Reprise Licence Manager の Report Log を解析

## 1.概要

Reprise Licence Manager の Report Log を解析するツールです。

チェックインのログは重複する場合があるため、重複ログ等に考慮して解析してます。

現状はかなり処理が思いです。

現状は、CSVとSQLiteが出力できます。


## 2.注意点

ファイルの出力項目等はまだ作成前


## 3.今後

マルチスレッドで処理してもそこまで早くならないので、原因解明中

解析したイベント(拒否系)の表示も作る予定です。


## 4.ライセンス

MIT license

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
