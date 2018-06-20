# PKUSeg：一个高准确度的中文分词工具包
PKUSeg简单易用，支持多领域分词，在不同领域的数据上都大幅提高了分词的准确率。
## 目录
* [项目介绍](#项目介绍)
* [使用方式](#使用方式)
* [各类分词工具包的性能对比](#各类分词工具包的性能对比)
* [相关论文](#相关论文)
* [作者](#作者)

## 项目介绍

PKUSeg由北京大学语言计算与机器学习研究组研制推出的一套全新的中文分词工具包。PKUSeg具有如下几个特点：

1. 准确率高。相比于传统的分词工具包，我们的工具包在不同领域的数据上都表现出了更好的分词准确度。
1. 多领域分词。我们训练了多种不同领域的分词模型。根据待分词的领域特点，用户可以自由地选择不同的模型。
3. 支持用户自训练模型。支持用户使用全新的标注的数据进行训练。

## 使用方式
    * 运行seg/bin/x64/Release目录下的seg.exe程序，windows用户可直接在cmd环境下，linux程序用户需要调用mono命令。
	* 分词模式：[mono] LSTM.exe test [-input file] [-output file]
    * 训练模式：[mono] LSTM.exe train [-train file] [-test file]
    * 从文本文件输入输出（注意均为UTF8文本）
### 参数说明
    test  分词
    train 训练
    [-input] 用户指定的待分词文件
    [-output] 用户指定的分词结果输出文件
    [-trainFile] & [-testFile] 用户标注的训练语料，句子之间以换行符分隔，词语之间以空格分隔
    
### 运行样例
	mono LSTM.exe test data/input.txt data/output.txt 		linux环境下进行分词
    LSTM.exe test data/input.txt data/output.txt 		windows 环境下进行分词
	mono LSTM.exe train data/train.txt data/test.txt 		根据指定的训练文件自定义训练，训练模型会保存到seg/bin/x64/Release/model目录下
	
### 预训练模型
分词模式下，用户需要在seg/bin/x64/Release/model/fast加载预训练好的模型，数据可以在https://drive.google.com/open?id=1BxFoqHtieVzLnjOt1xfya6wR-voSUhA网址下载，根据具体需要，用户可以自定义的选择不同的预训练模型。以下是对模型的说明：

```
msra：在MSRA（新闻语料）上训练的模型
ctb8： 在CTB8（新闻文本及网络文本的混合型语料）上训练的模型
weibo：在微博（网络文本语料）上训练的模型
```
MSRA数据由第二届国际汉语分词评测比赛提供（http://sighan.cs.uchicago.edu/bakeoff2005/ ），CTB8由LDC提供（https://catalog.ldc.upenn.edu/ldc2013t21   ），weibo数据由NLPCC分词比赛提供（http://tcci.ccf.org.cn/conference/2016/pages/page05_CFPTasks.html ）。


## 各类分词工具包的性能对比
我们选择THULAC、结巴分词国内代表分词软件与LancoSeg做性能比较。我们选择Windows作为测试环境，在新闻数据(MSRA)和混合型文本(CTB8)数据上对不同软件进行了速度和准确率测试。我们使用了第二届国际汉语分词评测比赛提供的分词评价脚本。评测结果如下：

MSRA

|Algorithm | Time | F-score|
|:------------|-------:|------------:|
| jieba | 0.82s |81.45 |
| THULAC | 7.12s | 85.48 |
| PKUSeg | 9.49s | 96.75 |

CTB8

|Algorithm | Time | F-score
|:------------|-------:|------------:|
|jieba|1.29s|79.58
|THULAC|5.15s|87.77
|PKUSeg|6.78s|95.64

## 开源协议
1. PKUSeg面向国内外大学、研究所、企业以及个人用于研究目的免费开放源代码。
2. 如有机构或个人拟将PKUSeg用于商业目的，请发邮件至xusun@pku.edu.cn洽谈技术许可协议。
3. 欢迎对该工具包提出任何宝贵意见和建议。请发邮件至jingjingxu@pku.edu.cn。

## 相关论文
* Xu Sun, Houfeng Wang, Wenjie Li. Fast Online Training with Frequency-Adaptive Learning Rates for Chinese Word Segmentation and New Word Detection. ACL. 253–262. 2012 
* Jingjing Xu, Xu Sun. Dependency-based Gated Recursive Neural Network for Chinese Word Segmentation. ACL 2016: 567-572

若使用此工具包，请引用如下文章：
```
@inproceedings{DBLP:conf/acl/SunWL12,
author = {Xu Sun and
Houfeng Wang and
Wenjie Li},
title = {Fast Online Training with Frequency-Adaptive Learning Rates for Chinese
Word Segmentation and New Word Detection},
booktitle = {The 50th Annual Meeting of the.pdf Association for Computational Linguistics, Proceedings of the Conference, July 8-14, 2012, Jeju Island, Korea- Volume 1: Long Papers},
pages = {253--262},
year = {2012}}

@inproceedings{DBLP:conf/acl/XuS16,
author = {Jingjing Xu and Xu Sun},
title = {Dependency-based Gated Recursive Neural Network for Chinese Word Segmentation},
booktitle = {Proceedings of the 54th Annual Meeting of the Association for Computational
Linguistics, {ACL} 2016, August 7-12, 2016, Berlin, Germany, Volume
2: Short Papers},
year = {2016}}
```
## 作者

Xu Sun （孙栩，导师）,  Jingjing Xu（许晶晶，博士生）
