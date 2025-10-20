using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Channels;

namespace HR_Economist
{
    
    public struct CvInfo  // 简历信息
    {
        public string Name { get; set; }  // 姓名
        public int DailyEconomy { get; set; }  // 每日赚钱
        public int DailyNegativeEnergy { get; set; }  // 每日增加负能量
        public int InitialTotalAbility { get; set; }  // 总能力
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            int Day = 1, EconomyGoal = 100000, NegativeEnergyGoal = 100000, totalEconomy = 0, totalNegativeEnergy = 0;
            int dailyTotalEconomy = 0, dailyTotalNegativeEnergy = 0;
            List<CvInfo> CVInfos = new List<CvInfo>();
            while (true)
            {
                Console.WriteLine($"第 {Day} 天");
                Console.WriteLine($"当前金钱目标：{totalEconomy}/{EconomyGoal}");
                Console.WriteLine($"当前负能量目标：{totalNegativeEnergy}/{NegativeEnergyGoal}");
                Random CVRandom = new Random();
                int CVNumber = CVRandom.Next(3);
                List<CvInfo> CVInfosTemp = new List<CvInfo>();
                for (int i = 0; i < CVNumber; i++)                              //生成随机简历
                {
                    CvInfo newCV = Person.PersonMaker();
                    CVInfosTemp.Add(newCV);
                }

                CVInfos.AddRange(CVInfosTemp);
                dailyTotalEconomy = CVInfos.Sum(e => e.DailyEconomy);               // 计算今日总收入
                dailyTotalNegativeEnergy = CVInfos.Sum(e => e.DailyNegativeEnergy);

                Console.WriteLine($"今日共收到 {CVNumber} 份简历：");
                Console.WriteLine($"今日收入{dailyTotalEconomy}元");
                Console.WriteLine($"今日获得负能量{dailyTotalNegativeEnergy}");
                Console.WriteLine("目前员工有：");
                foreach (var staff in CVInfos)
                {
                    Console.WriteLine(staff.Name);
                }

                totalEconomy += dailyTotalEconomy;                             //更新总收入
                totalNegativeEnergy += dailyTotalNegativeEnergy;

                if (totalEconomy >= EconomyGoal && totalNegativeEnergy >= NegativeEnergyGoal)
                {
                    Console.WriteLine($"恭喜你在第 {Day} 天达成目标！");
                    break;
                }
                Day++;
            }
        }

    }

    public class Person   // 人类
    {
        public static CvInfo PersonMaker()
        {
            // 使用共享 Random 来避免短时间内重复种子带来的重复值
            var rand = System.Random.Shared;
            return new CvInfo                     // 生成随机简历
            {
                Name = Names.GetRandomName(),
                InitialTotalAbility = 10000,
                DailyEconomy = rand.Next(100, 300),
                DailyNegativeEnergy = rand.Next(100, 300),
            };
        }
    }

    public static class Economy  //金钱类
    {

    }

    public static class NegativeEnergy  //负能量类
    {

    }

    public static class Names  //姓名类
    {
        
        private static string name = File.ReadAllText("name.txt");
        public static string[] names = name.Split('、');
        public static string GetRandomName()   //生成随机简历姓名
        {
            var namesArray = Names.names;
            Random NameRandom = new Random();
            int index = NameRandom.Next(namesArray.Length - 1);
            return namesArray[index];
        }
    }
}