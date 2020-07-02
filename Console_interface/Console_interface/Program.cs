using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_interface
{
    class Program
    {
        static void Main(string[] args)
        {
            #region interface
            phone ph = new iphone();

            ph.call_phone("0927-XXX");

            iphone iph = new iphone();

            iph.Siri("HI siri");
            #endregion

            #region abstract
            //Duck duck = new Duck(); // [錯誤]無法建立抽象類別的執行個體
            RubberDuck rubberDuck = new RubberDuck();
            Swan swan = new Swan();
            rubberDuck.Swim();
            rubberDuck.BelongTo();
            swan.Fly();
            swan.Quark();
            swan.Swim();
            swan.BelongTo();
 

            #endregion


            Console.Read();
        }
    }

    #region abstract

    public interface CanFly
    {
        void Fly();
    }

    public interface Quark
    {
        void Quark();
    }


    public abstract class Duck
    {
        public abstract void Swim();

        public void BelongTo()
        {
            Console.WriteLine("I belong to Duck.");
        }
    }

    // 塑膠鴨
    public class RubberDuck : Duck
    {
        public override void Swim()
        {
            Console.WriteLine("I am RubberDuck, I can Swim!");
        }
    }


    // 屬鴨科的天鵝
    // 只能繼承一個抽象類別，但可繼承很多介面
    public class Swan : Duck, CanFly, Quark
    {
        public void Fly()
        {
            Console.WriteLine("I am swam, I can fly!");
        }

        public void Quark()
        {
            Console.WriteLine("I am swam, I can quark!");
        }

        public override void Swim()
        {
            Console.WriteLine("I am swam, I can Swim!");
        }
    }
    #endregion abstract





    #region interface
    public interface phone
    {
        string call_phone(string phonenumber);
        string close_phone();
        string voice(int lev);
    }

    public class iphone : phone
    {
        public string call_phone(string num)
        {
            Console.WriteLine("iOS打電話給" + num);
            return "撥號中";
        }

        public string close_phone()
        {
            Console.WriteLine("iOS將通話中電話結束");
            return "結束通話";
        }

        public string voice(int lev)
        {
            Console.WriteLine("iOS將音量調整為" + lev);
            return "完成調音量";
        }

        public string Siri (string cmd)
        {
            //注意這是I手機沒有定義的方法
            return "Siri說" + cmd;
        }
    }

    #endregion
}
