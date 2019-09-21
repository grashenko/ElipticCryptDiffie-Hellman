using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ElipticCrypt
{

    class Program
    {
        public static string p = "6277101735386680763835789423207666416083908700390324961279"; //Модуль кривой

        public static long a = -3;// Параметр a уравнения
        public static string b = "2455155546008943817740293915197451784769108058161191238065";// Параметр b уравнения

        public static string Xg = "602046282375688656758213480587526111916698976636884684818";//Координаты точки 
        public static string Yg = "174050332293622031404857552280219410364023488927386650641";

        public static string n = "6277101735386680763835789423176059013767194773182842284081";// Порядок точки


        static void Main(string[] args)
        {

            Console.WriteLine("Введите случайное число q сторна A:");
            BigInteger ka = new BigInteger(Console.ReadLine(), 10);
            var pointG = new ECPoint();
            pointG.a = new BigInteger(a);
            pointG.b = new BigInteger(b, 10);
            pointG.x = new BigInteger(Xg, 10);
            pointG.y = new BigInteger(Yg, 10);
            pointG.FieldChar = new BigInteger(p, 10);

            var Ya =  ECPoint.multiply(ka, pointG);

            Console.WriteLine("Введите случайное число q сторна B:");
            BigInteger kb = new BigInteger(Console.ReadLine(), 10);
            var Yb = ECPoint.multiply(kb, pointG);

            var Ka = ECPoint.multiply(ka, Yb);
            var Kb = ECPoint.multiply(kb, Ya);

            Console.WriteLine("Ключ K у стороны A: " + " X: " + Ka.x + " Y: " + Ka.y);
            Console.WriteLine("Ключ K у стороны B: " + " X: " + Kb.x + " Y: " + Kb.y);
            Console.ReadLine();
            Console.WriteLine("Цифровая подпись ГОСТ ");

            Console.WriteLine("Введите сообщение: ");
            string message = Console.ReadLine();
            GOST hash = new GOST(256);
            byte[] H = hash.GetHash(Encoding.Default.GetBytes(message));
            var gost = new DSGost(new BigInteger(p, 10), new BigInteger(a), new BigInteger(b, 10), new BigInteger(n, 10), pointG);

            ECPoint Q = gost.GenPublicKey(kb);
            var podpis = gost.SingGen
                (H, kb);

            Console.WriteLine("Подпись: " + podpis);
            var isVerivicated = gost.SingVer(H, podpis, Q);
            if (isVerivicated)
            {
                Console.WriteLine("Подпись верна");
            }
            else
            {
                Console.WriteLine("Подпись не верна");
            }

            Console.ReadLine();
        }

    }
}
