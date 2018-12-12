using Data.Model;
using Data.Repository;
using isRock.LineBot;
using isRock.LineBot.Conversation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LineBotAPI.Controllers
{
    public class WebHookController : isRock.LineBot.LineWebHookControllerBase
    {

        private LifeRepository _LifeRepo = new LifeRepository();
        private LoveRepository _LoveRepo = new LoveRepository();
        private SoulRepository _SoulRepo = new SoulRepository();
        private DISCRepository _DISCRepo = new DISCRepository();
        static Dictionary<string, int> dicDISCCount;
        string responseMsg = "";

        [Route("api/WebHook")]
        [HttpPost]
        public IHttpActionResult POST()
        {
            try
            {
                var author = "";
                var message = "";




                //建立actions，作為ButtonTemplate的用戶回覆行為
                var actions = new List<isRock.LineBot.TemplateActionBase>();

                //定義資訊蒐集者
                isRock.LineBot.Conversation.InformationCollector<LeaveRequest> CIC =
                    new isRock.LineBot.Conversation.InformationCollector<LeaveRequest>(ChannelAccessToken);
                //定義接收CIC結果的類別
                ProcessResult<LeaveRequest> CICresult;

                //設定ChannelAccessToken(或抓取Web.Config)
                this.ChannelAccessToken = Properties.Settings.Default["ChannelAccessToken"].ToString();

                this.PushMessage("U849bacfde38571008bbcb71d1f0017a9", "WWWWWWW");
                //取得Line Event
                isRock.LineBot.Event LineEvent = this.ReceivedMessage.events.FirstOrDefault();

                //Uid
                string Uid = LineEvent.source.userId;

                if (LineEvent.message.text == "人生")
                {
                    var windom = _LifeRepo.Get();
                    message = windom.message;
                    author = "———" + windom.author;
                    responseMsg = message + author;

                    this.ReplyMessage(LineEvent.replyToken, responseMsg);
                }
                else if (LineEvent.message.text == "愛情")
                {
                    var windom = _LoveRepo.Get();
                    message = windom.message;
                    author = "———" + windom.author;
                    responseMsg = message + author;


                    this.ReplyMessage(LineEvent.replyToken, responseMsg);
                }
                else if (LineEvent.message.text == "心靈")
                {
                    var windom = _SoulRepo.Get();
                    message = windom.message;
                    responseMsg = message;


                    this.ReplyMessage(LineEvent.replyToken, responseMsg);
                }
                else
                {

                    if (ReceivedMessage.events[0].message.text.IndexOf("DISC") >= 0)
                    {


                        dicDISCCount = new Dictionary<string, int>()
                        {
                            {"A",0}, {"B",0}, {"C",0}, {"D",0}
                        };

                        //把訊息丟給CIC 
                        CICresult = CIC.Process(ReceivedMessage.events[0], true);
                        this.PushMessage(Uid, "請在30秒內回答以下問題");
      
                        responseMsg = "開始測驗\n";

                        switch (CICresult.ProcessResultStatus)
                        {
                            case ProcessResultStatus.Processed:

                                //ResponseButtonsTemplateCandidate的部分
                                if (CICresult.ResponseButtonsTemplateCandidate != null)
                                {
                                    //如果有template Message，直接回覆，否則放到後面一起回覆
                                    isRock.LineBot.Utility.ReplyTemplateMessage(
                                        ReceivedMessage.events[0].replyToken,
                                        CICresult.ResponseButtonsTemplateCandidate,
                                        ChannelAccessToken);
                                    return Ok();
                                }

                                //取得候選訊息發送
                                responseMsg += CICresult.ResponseMessageCandidate;
                                break;
                            case ProcessResultStatus.Done:
                                responseMsg += CICresult.ResponseMessageCandidate;
                                responseMsg += $"蒐集到的資料有...\n";
                                responseMsg += Newtonsoft.Json.JsonConvert.SerializeObject(CICresult.ConversationState.ConversationEntity);
                                break;
                            case ProcessResultStatus.Pass:
                                responseMsg = $"你說的 '{ReceivedMessage.events[0].message.text}' 我看不懂，如果想要請假，請跟我說 : 『我要請假』";
                                break;
                            case ProcessResultStatus.Exception:
                                //取得候選訊息發送
                                responseMsg += CICresult.ResponseMessageCandidate;
                                break;
                            case ProcessResultStatus.Break:
                                //取得候選訊息發送
                                responseMsg += CICresult.ResponseMessageCandidate;
                                break;
                            case ProcessResultStatus.InputDataFitError:
                                responseMsg += "\n資料型態不合\n";
                                responseMsg += CICresult.ResponseMessageCandidate;
                                break;
                            default:
                                //取得候選訊息發送
                                responseMsg += CICresult.ResponseMessageCandidate;
                                break;
                        }

                        isRock.LineBot.Utility.ReplyMessage(ReceivedMessage.events[0].replyToken, responseMsg, ChannelAccessToken);
                    }
                    else
                    {

                        CIC.OnMessageTypeCheck += (s, e) =>
                        {
                            if (e.CurrentPropertyName.IndexOf("Question") >= 0)
                            {
                                string key = e.ReceievedMessage.Substring(0, 1);
                                dicDISCCount[key]++;
                            }
                        };

                        //把訊息丟給CIC 
                        CICresult = CIC.Process(ReceivedMessage.events[0]);

                        switch (CICresult.ProcessResultStatus)
                        {
                            case ProcessResultStatus.Processed:

                                //ResponseButtonsTemplateCandidate的部分
                                if (CICresult.ResponseButtonsTemplateCandidate != null)
                                {
                                    //如果有template Message，直接回覆，否則放到後面一起回覆
                                    isRock.LineBot.Utility.ReplyTemplateMessage(
                                        ReceivedMessage.events[0].replyToken,
                                        CICresult.ResponseButtonsTemplateCandidate,
                                        ChannelAccessToken);
                                    return Ok();
                                }

                                //取得候選訊息發送
                                responseMsg += CICresult.ResponseMessageCandidate;
                                break;
                            case ProcessResultStatus.Done:

                                switch (dicDISCCount.Aggregate((x, y) => x.Value > y.Value ? x : y).Key)
                                {
                                    case "A":
                                        this.PushMessage(Uid, "你是：孔雀 (影響型)：外向 多言 樂觀");
                                        this.PushMessage(Uid, "特點：一群人裡面說話最多的 天生希望成為注意力的中心，具有很強的好奇心 熱情 熱心具有表達能力 精力充沛 具有幹勁（但是卻缺乏毅力 所以常常這幹幹 那乾幹） 好表現 粗線條 輕許諾（因為熱心所以常常答應別人 但是由於記憶差 所以常常答應後就忘記了） 以自己的快樂為主");
                                        this.PushMessage(Uid, "缺點：以自己為中心 獨霸主題 愛打斷別人的談話 不注意記憶 變化無常 這類人易交朋友 但深切的朋友卻不多 喜好多卻不精 缺乏毅力");
                                        this.ReplyMessage(ReceivedMessage.events[0].replyToken, "切入點：如果跟這類型的人交往 一定要多誇獎他 多鼓勵他 多給他說話的機會");
                                        break;
                                    case "B":
                                        this.PushMessage(Uid, "你是：老虎 (支配型) : 外向 行動者 樂觀");
                                        this.PushMessage(Uid, "特點：喜歡做主 行動力強 行動速度 思考力稍弱 喜歡做目標 不達目的不罷休 充滿自信 意志堅定 有活力 做事主動 不易氣餒 是推動別人行動的人 粗線條 不容易適應環境 （不過由於行動力很強 所以往往做事會有很大成就) ");
                                        this.PushMessage(Uid, "缺點：不易看到別人的需求，只看到自己的需求 做錯事後很容易原諒自己 固執 易爭吵 好鬥 說話極易傷害別人 具有強迫性 很容易支配別人 無耐性 專橫 經常人際關係差 （這類人總覺得自己是對的 不太需要朋友，並且這類人天生行動力強 但是即使是正確的事情 也因為性格問題 說話傷害到別人 而得不到別人的支持和認同）");
                                        this.ReplyMessage(ReceivedMessage.events[0].replyToken, "注意點：這類型嚴重者會很獨斷 霸道 容易讓別人感到壓力 相處很累");
                                        break;
                                    case "C":
                                        this.PushMessage(Uid, "你是：貓頭鷹 (分析型)：內向 思考者 對事物看法較為負面");
                                        this.PushMessage(Uid, "特點：以思考為主 深思熟慮 嚴肅 有目標 並且目標感很強 追求完美 有藝術天分 沉悶 關注細節 完美主義 高標準 想得多但做得少 做事前一定要先想個計劃 有條理 有組織 交友慎重（但一旦交往 就會很忠誠的對待朋友） 關心別人 為別人犧牲是自己的意願 （所以這類型的人一生一定有幾個特別好的朋友一輩子的朋友那種）情感豐富容易感動 也容易受傷 高標準–對自己要求高對​​別人要求也高 希望一切都作的很好 很對 、理想主義 朝著自己的目標前進");
                                        this.PushMessage(Uid, "缺點：行動力弱 優柔寡斷 容易抑鬱（常常是因為要求過高了 當達不到時候就會很失望）容易自慚自愧 悲觀 天生消極 易受環境影響 情緒化 注意點：這類型的人太容易思考 過分時會情緒低落");
                                        this.ReplyMessage(ReceivedMessage.events[0].replyToken, "切入點：如果想和著類型的人合作 一定要先打動他 但是不要急功近利 要一點一點的建立信任和感情 這類型的人 一旦認同你後 會很忠誠 忠心");
                                        break;
                                    case "D":
                                        this.PushMessage(Uid, "你是：無尾熊(穩健型)：內向 執行者 追隨者");
                                        this.PushMessage(Uid, "特點：性格低調 易相處 很輕鬆 平和 無異議 耐心 適應力強 無攻擊性 很好的聆聽者 人際關係好（和事老） 所以朋友很多 不愛生氣，一但答應下的工作會默默做完");
                                        this.PushMessage(Uid, "缺點：不容易興奮 拒絕改變 喜歡一成不變 目標感不強 看似懶惰 不願承擔責任 迴避壓力 沉默、馬虎 無主見（需要力量型的人 給於指導，但不要施加壓力） 不善於做決定");
                                        this.PushMessage(Uid, "切入點：和這類型的人交往 一定要鼓勵他 促進他");
                                        this.ReplyMessage(ReceivedMessage.events[0].replyToken, "注意點：這類型過分時會毫無主見 做事漫不經心");
                                        break;
                                }
                                return Ok();
                                //responseMsg += CICresult.ResponseMessageCandidate;
                                //responseMsg += $"蒐集到的資料有...\n";
                                //responseMsg += Newtonsoft.Json.JsonConvert.SerializeObject(CICresult.ConversationState.ConversationEntity);
                                break;
                            case ProcessResultStatus.Pass:
                                //回音機器人
                                responseMsg = LineEvent.message.text;
                                this.ReplyMessage(LineEvent.replyToken, "我是回音機器人--你說了[" + responseMsg + "]");
                                //responseMsg = $"你說的 '{ReceivedMessage.events[0].message.text}' 我看不懂，如果想要請假，請跟我說 : 『我要請假』";
                                break;
                            case ProcessResultStatus.Exception:
                                //取得候選訊息發送
                                responseMsg += CICresult.ResponseMessageCandidate;
                                break;
                            case ProcessResultStatus.Break:
                                //取得候選訊息發送
                                responseMsg += CICresult.ResponseMessageCandidate;
                                break;
                            case ProcessResultStatus.InputDataFitError:
                                responseMsg += "\n資料型態不合\n";
                                responseMsg += CICresult.ResponseMessageCandidate;
                                break;
                            default:
                                //取得候選訊息發送
                                responseMsg += CICresult.ResponseMessageCandidate;
                                break;
                        }
                        isRock.LineBot.Utility.ReplyMessage(ReceivedMessage.events[0].replyToken, responseMsg, ChannelAccessToken);



                    }

                }

                return Ok();
            }
            catch (Exception ex)
            {
                //回覆訊息
                this.PushMessage("!!!改成你的AdminUserId!!!", "發生錯誤:\n" + ex.Message);
                //response OK
                return Ok();
            }
        }



        public class LeaveRequest : ConversationEntity
        {
            [ButtonsTemplateQuestion("挑選一個與您最相近的形容詞", "共40題，已完成0題", "https://arock.blob.core.windows.net/blogdata201706/22-124357-ad3c87d6-b9cc-488a-8150-1c2fe642d237.png", "A.活潑生動 ", "B. 富於冒險", " C. 善於分析", "D. 適應性強")]
            [Order(1)]
            public string Question_1 { get; set; }
            [ButtonsTemplateQuestion("挑選一個與您最相近的形容詞", "共40題，已完成1題", "https://arock.blob.core.windows.net/blogdata201706/22-124357-ad3c87d6-b9cc-488a-8150-1c2fe642d237.png", "A.喜好娛樂 ", "B. 善於說服 ", "C. 堅持不懈 ", "D. 平和")]
            [Order(2)]
            public string Question_2 { get; set; }
            [ButtonsTemplateQuestion("挑選一個與您最相近的形容詞", "共40題，已完成2題", "https://arock.blob.core.windows.net/blogdata201706/22-124357-ad3c87d6-b9cc-488a-8150-1c2fe642d237.png", "A.善於社交 ", "B. 意志堅定 ", "C. 自我犧牲 ", "D. 較少爭辯")]
            [Order(3)]
            public string Question_3 { get; set; }
            [ButtonsTemplateQuestion("挑選一個與您最相近的形容詞", "共40題，已完成3題", "https://arock.blob.core.windows.net/blogdata201706/22-124357-ad3c87d6-b9cc-488a-8150-1c2fe642d237.png", "A.使人認同 ", "B. 喜競爭勝 ", "C. 體貼 ", "D. 自控性好")]
            [Order(4)]
            public string Question_4 { get; set; }
            [ButtonsTemplateQuestion("挑選一個與您最相近的形容詞", "共40題，已完成4題", "https://arock.blob.core.windows.net/blogdata201706/22-124357-ad3c87d6-b9cc-488a-8150-1c2fe642d237.png", "A.使人振作 ", "B. 善於應變 ", "C. 令人尊敬 ", "D. 含蓄")]
            [Order(5)]
            public string Question_5 { get; set; }
            [ButtonsTemplateQuestion("挑選一個與您最相近的形容詞", "共40題，已完成5題", "https://arock.blob.core.windows.net/blogdata201706/22-124357-ad3c87d6-b9cc-488a-8150-1c2fe642d237.png", "A.生機勃勃 ", "B. 自立 ", "C. 敏感 ", "D. 滿足")]
            [Order(6)]
            public string Question_6 { get; set; }
            [ButtonsTemplateQuestion("挑選一個與您最相近的形容詞", "共40題，已完成6題", "https://arock.blob.core.windows.net/blogdata201706/22-124357-ad3c87d6-b9cc-488a-8150-1c2fe642d237.png", "A.推動者 ", "B. 積極 ", "C. 計劃者 ", "D. 耐性")]
            [Order(7)]
            public string Question_7 { get; set; }
            [ButtonsTemplateQuestion("挑選一個與您最相近的形容詞", "共40題，已完成7題", "https://arock.blob.core.windows.net/blogdata201706/22-124357-ad3c87d6-b9cc-488a-8150-1c2fe642d237.png", "A.無拘無束 ", "B. 肯定 ", "C. 時間性 ", "D. 羞澀")]
            [Order(8)]
            public string Question_8 { get; set; }
            [ButtonsTemplateQuestion("挑選一個與您最相近的形容詞", "共40題，已完成8題", "https://arock.blob.core.windows.net/blogdata201706/22-124357-ad3c87d6-b9cc-488a-8150-1c2fe642d237.png", "A.樂觀 ", "B. 坦率 ", "C. 井井有條 ", "D. 遷就")]
            [Order(9)]
            public string Question_9 { get; set; }
            [ButtonsTemplateQuestion("挑選一個與您最相近的形容詞", "共40題，已完成9題", "https://arock.blob.core.windows.net/blogdata201706/22-124357-ad3c87d6-b9cc-488a-8150-1c2fe642d237.png", "A. 有趣 ", "B. 強迫性 ", "C. 忠誠 ", "D. 友善")]
            [Order(10)]
            public string Question_10 { get; set; }
            [ButtonsTemplateQuestion("挑選一個與您最相近的形容詞", "共40題，已完成10題", "https://arock.blob.core.windows.net/blogdata201706/22-124357-ad3c87d6-b9cc-488a-8150-1c2fe642d237.png", "A. 可愛 ", "B. 勇敢 ", "C. 注意細節 ", "D. 外交手腕")]
            [Order(11)]
            public string Question_11 { get; set; }
            [ButtonsTemplateQuestion("挑選一個與您最相近的形容詞", "共40題，已完成11題", "https://arock.blob.core.windows.net/blogdata201706/22-124357-ad3c87d6-b9cc-488a-8150-1c2fe642d237.png", "A. 讓人高興 ", "B. 自信 ", "C. 文化修養 ", "D. 貫​​徹始終")]
            [Order(12)]
            public string Question_12 { get; set; }
            [ButtonsTemplateQuestion("挑選一個與您最相近的形容詞", "共40題，已完成12題", "https://arock.blob.core.windows.net/blogdata201706/22-124357-ad3c87d6-b9cc-488a-8150-1c2fe642d237.png", "A. 富激勵性 ", "B. 獨立 ", "C. 理想主義 ", "D. 無攻擊性")]
            [Order(13)]
            public string Question_13 { get; set; }
            [ButtonsTemplateQuestion("挑選一個與您最相近的形容詞", "共40題，已完成13題", "https://arock.blob.core.windows.net/blogdata201706/22-124357-ad3c87d6-b9cc-488a-8150-1c2fe642d237.png", "A. 情感外露 ", "B. 果斷 ", "C. 深沉 ", "D. 淡然幽默")]
            [Order(14)]
            public string Question_14 { get; set; }
            [ButtonsTemplateQuestion("挑選一個與您最相近的形容詞", "共40題，已完成14題", "https://arock.blob.core.windows.net/blogdata201706/22-124357-ad3c87d6-b9cc-488a-8150-1c2fe642d237.png", "A. 喜交朋友 ", "B. 發起者 ", "C. 音樂性 ", "D. 調解者")]
            [Order(15)]
            public string Question_15 { get; set; }
            [ButtonsTemplateQuestion("挑選一個與您最相近的形容詞", "共40題，已完成15題", "https://arock.blob.core.windows.net/blogdata201706/22-124357-ad3c87d6-b9cc-488a-8150-1c2fe642d237.png", "A. 多言 ", "B. 執著 ", "C. 考慮周到 ", "D. 容忍")]
            [Order(16)]
            public string Question_16 { get; set; }
            [ButtonsTemplateQuestion("挑選一個與您最相近的形容詞", "共40題，已完成16題", "https://arock.blob.core.windows.net/blogdata201706/22-124357-ad3c87d6-b9cc-488a-8150-1c2fe642d237.png", "A. 活力充沛 ", "B. 領導者 ", "C. 忠心 ", "D. 聆聽著")]
            [Order(17)]
            public string Question_17 { get; set; }
            [ButtonsTemplateQuestion("挑選一個與您最相近的形容詞", "共40題，已完成17題", "https://arock.blob.core.windows.net/blogdata201706/22-124357-ad3c87d6-b9cc-488a-8150-1c2fe642d237.png", "A. 讓人喜愛 ", "B. 首領 ", "C. 製圖者 ", "D. 知足")]
            [Order(18)]
            public string Question_18 { get; set; }
            [ButtonsTemplateQuestion("挑選一個與您最相近的形容詞", "共40題，已完成18題", "https://arock.blob.core.windows.net/blogdata201706/22-124357-ad3c87d6-b9cc-488a-8150-1c2fe642d237.png", "A. 受歡迎 ", "B. 勤勞 ", "C. 完美主義者 ", "D. 和氣")]
            [Order(19)]
            public string Question_19 { get; set; }
            [ButtonsTemplateQuestion("挑選一個與您最相近的形容詞", "共40題，已完成19題", "https://arock.blob.core.windows.net/blogdata201706/22-124357-ad3c87d6-b9cc-488a-8150-1c2fe642d237.png", "A. 跳躍型 ", "B. 無畏 ", "C. 規範型 ", "D. 平衡")]
            [Order(20)]
            public string Question_20 { get; set; }
            [ButtonsTemplateQuestion("挑選一個與您最相近的形容詞", "共40題，已完成20題", "https://arock.blob.core.windows.net/blogdata201706/22-124357-ad3c87d6-b9cc-488a-8150-1c2fe642d237.png", "A. 露骨 ", "B. 專橫 ", "C. 乏味 ", "D. 扭捏")]
            [Order(21)]
            public string Question_21 { get; set; }
            [ButtonsTemplateQuestion("挑選一個與您最相近的形容詞", "共40題，已完成21題", "https://arock.blob.core.windows.net/blogdata201706/22-124357-ad3c87d6-b9cc-488a-8150-1c2fe642d237.png", "A. 散漫 ", "B. 缺乏同情心 ", "C. 不寬恕 ", "D. 缺乏熱情")]
            [Order(22)]
            public string Question_22 { get; set; }
            [ButtonsTemplateQuestion("挑選一個與您最相近的形容詞", "共40題，已完成22題", "https://arock.blob.core.windows.net/blogdata201706/22-124357-ad3c87d6-b9cc-488a-8150-1c2fe642d237.png", "A. 嘮叨 ", "B. 逆反 ", "C. 怨恨 ", "D. 保留")]
            [Order(23)]
            public string Question_23 { get; set; }
            [ButtonsTemplateQuestion("挑選一個與您最相近的形容詞", "共40題，已完成23題", "https://arock.blob.core.windows.net/blogdata201706/22-124357-ad3c87d6-b9cc-488a-8150-1c2fe642d237.png", "A. 健忘 ", "B. 率直 ", "C. 挑剔 ", "D. 膽小")]
            [Order(24)]
            public string Question_24 { get; set; }
            [ButtonsTemplateQuestion("挑選一個與您最相近的形容詞", "共40題，已完成24題", "https://arock.blob.core.windows.net/blogdata201706/22-124357-ad3c87d6-b9cc-488a-8150-1c2fe642d237.png", "A. 好插口 ", "B. 沒耐性 ", "C. 優柔寡斷 ", "D. 無安全感")]
            [Order(25)]
            public string Question_25 { get; set; }
            [ButtonsTemplateQuestion("挑選一個與您最相近的形容詞", "共40題，已完成25題", "https://arock.blob.core.windows.net/blogdata201706/22-124357-ad3c87d6-b9cc-488a-8150-1c2fe642d237.png", "A. 難預測 ", "B. 直截了當 ", "C. 過於嚴肅 ", "D. 不參與")]
            [Order(26)]
            public string Question_26 { get; set; }
            [ButtonsTemplateQuestion("挑選一個與您最相近的形容詞", "共40題，已完成26題", "https://arock.blob.core.windows.net/blogdata201706/22-124357-ad3c87d6-b9cc-488a-8150-1c2fe642d237.png", "A. 即興 ", "B. 固執 ", "C. 難於取悅 ", "D. 猶豫不決")]
            [Order(27)]
            public string Question_27 { get; set; }
            [ButtonsTemplateQuestion("挑選一個與您最相近的形容詞", "共40題，已完成27題", "https://arock.blob.core.windows.net/blogdata201706/22-124357-ad3c87d6-b9cc-488a-8150-1c2fe642d237.png", "A. 放任 ", "B. 自負 ", "C. 悲觀 ", "D. 平淡")]
            [Order(28)]
            public string Question_28 { get; set; }
            [ButtonsTemplateQuestion("挑選一個與您最相近的形容詞", "共40題，已完成28題", "https://arock.blob.core.windows.net/blogdata201706/22-124357-ad3c87d6-b9cc-488a-8150-1c2fe642d237.png", "A. 易怒 ", "B. 好爭吵 ", "C. 孤芳自賞 ", "D. 無目標")]
            [Order(29)]
            public string Question_29 { get; set; }
            [ButtonsTemplateQuestion("挑選一個與您最相近的形容詞", "共40題，已完成29題", "https://arock.blob.core.windows.net/blogdata201706/22-124357-ad3c87d6-b9cc-488a-8150-1c2fe642d237.png", "A. 天真 ", "B. 魯莽 ", "C. 消極 ", "D. 冷漠")]
            [Order(30)]
            public string Question_30 { get; set; }
            [ButtonsTemplateQuestion("挑選一個與您最相近的形容詞", "共40題，已完成30題", "https://arock.blob.core.windows.net/blogdata201706/22-124357-ad3c87d6-b9cc-488a-8150-1c2fe642d237.png", "A. 喜獲認同 ", "B. 工作狂 ", "C. 不善交際 ", "D. 擔憂")]
            [Order(31)]
            public string Question_31 { get; set; }
            [ButtonsTemplateQuestion("挑選一個與您最相近的形容詞", "共40題，已完成31題", "https://arock.blob.core.windows.net/blogdata201706/22-124357-ad3c87d6-b9cc-488a-8150-1c2fe642d237.png", "A. 喋喋不休 ", "B. 不圓滑老練 ", "C. 過分敏感 ", "D. 膽怯")]
            [Order(32)]
            public string Question_32 { get; set; }
            [ButtonsTemplateQuestion("挑選一個與您最相近的形容詞", "共40題，已完成32題", "https://arock.blob.core.windows.net/blogdata201706/22-124357-ad3c87d6-b9cc-488a-8150-1c2fe642d237.png", "A. 雜亂無章 ", "B. 跋扈 ", "C. 抑鬱 ", "D. 靦腆")]
            [Order(33)]
            public string Question_33 { get; set; }
            [ButtonsTemplateQuestion("挑選一個與您最相近的形容詞", "共40題，已完成33題", "https://arock.blob.core.windows.net/blogdata201706/22-124357-ad3c87d6-b9cc-488a-8150-1c2fe642d237.png", "A. 缺乏毅力 ", "B. 不容忍 ", "C. 內向 ", "D. 無異議")]
            [Order(34)]
            public string Question_34 { get; set; }
            [ButtonsTemplateQuestion("挑選一個與您最相近的形容詞", "共40題，已完成34題", "https://arock.blob.core.windows.net/blogdata201706/22-124357-ad3c87d6-b9cc-488a-8150-1c2fe642d237.png", "A. 零亂 ", "B. 喜操縱 ", "C. 情緒化 ", "D. 喃喃自語")]
            [Order(35)]
            public string Question_35 { get; set; }
            [ButtonsTemplateQuestion("挑選一個與您最相近的形容詞", "共40題，已完成35題", "https://arock.blob.core.windows.net/blogdata201706/22-124357-ad3c87d6-b9cc-488a-8150-1c2fe642d237.png", "A. 好表現 ", "B. 頑固 ", "C. 有戒心 ", "D. 緩慢")]
            [Order(36)]
            public string Question_36 { get; set; }
            [ButtonsTemplateQuestion("挑選一個與您最相近的形容詞", "共40題，已完成36題", "https://arock.blob.core.windows.net/blogdata201706/22-124357-ad3c87d6-b9cc-488a-8150-1c2fe642d237.png", "A. 大嗓門 ", "B. 統治欲 ", "C. 孤僻 ", "D. 懶惰")]
            [Order(37)]
            public string Question_37 { get; set; }
            [ButtonsTemplateQuestion("挑選一個與您最相近的形容詞", "共40題，已完成37題", "https://arock.blob.core.windows.net/blogdata201706/22-124357-ad3c87d6-b9cc-488a-8150-1c2fe642d237.png", "A. 不專注 ", "B. 易怒 ", "C. 多疑 ", "D. 拖延")]
            [Order(38)]
            public string Question_38 { get; set; }
            [ButtonsTemplateQuestion("挑選一個與您最相近的形容詞", "共40題，已完成38題", "https://arock.blob.core.windows.net/blogdata201706/22-124357-ad3c87d6-b9cc-488a-8150-1c2fe642d237.png", "A. 煩躁 ", "B. 輕率 ", "C. 報復型 ", "D. 勉強")]
            [Order(39)]
            public string Question_39 { get; set; }
            [ButtonsTemplateQuestion("挑選一個與您最相近的形容詞", "共40題，已完成39題", "https://arock.blob.core.windows.net/blogdata201706/22-124357-ad3c87d6-b9cc-488a-8150-1c2fe642d237.png", "A. 善變 ", "B. 狡猾 ", "C. 好批評 ", "D. 妥協")]
            [Order(40)]
            public string Question_40 { get; set; }
        }
    }

    //public UserInfoModel GetInfo(isRock.LineBot.Event ev)
    //{
    //    UserInfoModel User = new UserInfoModel();

    //    User.Id = ev.source.userId;
    //    User.ReplyToken = ev.replyToken;
    //    User.Timestamp = (new DateTime(1970, 1, 1, 0, 0, 0)).AddHours(8).AddSeconds(ev.timestamp);

    //    return User;
    //}

    //public class DISCModel
    //{
    //    public string QuestionTitle { get; set; }
    //}

    public class UserInfoModel
    {

        /// <summary>
        /// 唯一碼
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ReplyToken { get; set; }

        /// <summary>
        /// 最後登入時間戳
        /// </summary>
        public DateTime Timestamp { get; set; }

    }

}


