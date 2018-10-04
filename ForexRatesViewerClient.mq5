//+------------------------------------------------------------------+
//|                                       ForexRatesViewerClient.mq5 |
//|                                Copyright 2018, InvestDataSystems |
//|                 https://tradingbot.wixsite.com/robots-de-trading |
//+------------------------------------------------------------------+
#property copyright "Copyright 2018, InvestDataSystems"
#property link      "https://tradingbot.wixsite.com/robots-de-trading"
#property version   "1.00"
//+------------------------------------------------------------------+
//| Expert initialization function                                   |
//+------------------------------------------------------------------+
int OnInit()
  {

//---
   return(INIT_SUCCEEDED);
  }

char post[];
char result[];
string cookie=NULL,headers;
string url="http://127.0.0.1:9090/send_data/";
void SendData(string strpost);
int i=0;
int res;
//+------------------------------------------------------------------+
//|                                                                  |
//+------------------------------------------------------------------+
void SendData(string strpost)
  {
//---
   ResetLastError();
   ArrayResize(post,StringLen(strpost));
   for(i=0; i<StringLen(strpost); i++)
     {
      post[i]=strpost[i];
     }
   res=WebRequest("POST",url,cookie,NULL,500,post,0,result,headers);
  }
//+------------------------------------------------------------------+
//| Expert deinitialization function                                 |
//+------------------------------------------------------------------+
void OnDeinit(const int reason)
  {
//---

  }
//+------------------------------------------------------------------+
//| Expert tick function                                             |
//+------------------------------------------------------------------+
MqlTick last_tick;
int stotal;
int sindex;
string sname;
void OnTick()
  {
   int stotal=SymbolsTotal(false); // seulement les symboles dans le marketwatch (false)
   for(sindex=0; sindex<stotal; sindex++)
     {
      sname=SymbolName(sindex, false);
      if(SymbolInfoTick(sname,last_tick))
        {
         // Time, Symbol, Bid, Ask, Vol
         SendData(last_tick.time+";"+sname+";"+last_tick.bid+";"+last_tick.ask+";"+last_tick.volume);
        }
      else Print("SymbolInfoTick() failed, error = ",GetLastError());
     }
  }
//+------------------------------------------------------------------+
