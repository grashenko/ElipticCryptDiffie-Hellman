using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElipticCrypt
{
    public class DSGost
    {
        private BigInteger p = new BigInteger();
        private BigInteger a = new BigInteger();
        private BigInteger b = new BigInteger();
        private BigInteger n = new BigInteger();
        private byte[] xG;
        private ECPoint G = new ECPoint();
        
        public DSGost(BigInteger p, BigInteger a, BigInteger b, BigInteger n, ECPoint G)
        {
            this.a = a;
            this.b = b;
            this.n = n;
            this.p = p;
            this.G = G;
        }


        //С помощью секретного ключа d вычисляем точку Q=d*G, это и будет наш публичный ключ
        public ECPoint GenPublicKey(BigInteger d)
        {
            ECPoint Q = ECPoint.multiply(d, G);
            return Q;
        }


        //функция вычисления квадратоного корня по модулю простого числа q
      
        //подписываем сообщение
        public string SingGen(byte[] h, BigInteger d)
        {
            BigInteger alpha = new BigInteger(h);
            BigInteger e = alpha % n;
            if (e == 0)
                e = 1;
            BigInteger k = new BigInteger();
            ECPoint C=new ECPoint();
            BigInteger r=new BigInteger();
            BigInteger s = new BigInteger();
            do
            {
                do
                {
                    k.genRandomBits(n.bitCount(), new Random());
                } while ((k < 0) || (k > n));
                C = ECPoint.multiply(k, G);
                r = C.x % n;
                s = ((r * d) + (k * e)) % n;
            } while ((r == 0)||(s==0));
            string Rvector = padding(r.ToHexString(),n.bitCount()/4);
            string Svector = padding(s.ToHexString(), n.bitCount() / 4);
            return Rvector + Svector;
        }

        //проверяем подпись 
        public bool SingVer(byte[] H, string sing, ECPoint Q)
        {
            string Rvector = sing.Substring(0, n.bitCount() / 4);
            string Svector = sing.Substring(n.bitCount() / 4, n.bitCount() / 4);
            BigInteger r = new BigInteger(Rvector, 16);
            BigInteger s = new BigInteger(Svector, 16);
            if ((r < 1) || (r > (n - 1)) || (s < 1) || (s > (n - 1)))
                return false;
            BigInteger alpha = new BigInteger(H);
            BigInteger e = alpha % n;
            if (e == 0)
                e = 1;
            BigInteger v = e.modInverse(n);
            BigInteger z1 = (s * v) % n;
            BigInteger z2 = n + ((-(r * v)) % n);
            ECPoint A = ECPoint.multiply(z1, G);
            ECPoint B = ECPoint.multiply(z2, Q);
            ECPoint C = A + B;
            BigInteger R = C.x % n;
            if (R == r)
                return true;
            else
                return false;
        }

        //дополняем подпись нулями слева до длины n, где n - длина модуля в битах
        private string padding(string input, int size)
        {
            if (input.Length < size)
            {
                do
                {
                    input = "0" + input;
                } while (input.Length < size);
            }
            return input;
        }
    }
}
