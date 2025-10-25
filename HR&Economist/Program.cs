using System.ComponentModel;
using System.Configuration.Assemblies;
using System.Security;

namespace HR_Economist
{
    public struct CV_Info  //简历结构
    {
        public string Name;  //姓名
        public int selfAbility1;  //能力值1(每人两种能力)
        public int selfAbility2;  //能力值2(每人两种能力)
        public int selfWork; //工作
        public int WorkIncome; //工作收入
        public int NegativeEnergyIncome;   //负能量收入
        public bool IsWorking;  //是否在职
    }
    public enum Abilities   //能力种类
    {
        /*
        0 食物获取与生产
        1 居所建造
        2 工艺与制造
        3 医疗
        4 防御与安全
        5 探索与开拓
        */
        FoodGetter_Producer = 0,
        House_Builder = 2,
        Craft_Manufacturer = 4,
        Medical = 6,
        Defense_Security = 8,
        Explore_Developer = 10
    };
    public enum Works   //工作种类
    {
        FoodGetter,
        Producer,
        House,
        Builder,
        Craft,
        Manufacturer,
        Medical,
        Medicine,
        Defense,
        Security,
        Explore,
        Developer
    };
    internal class Texting   //测试程序(入口,正式应用时许注释掉)
    {
        static void Main(string[] args)
        {
            int Day = 1;
            int CVNumber = 0;
            List<CV_Info> CVInfos = new List<CV_Info>();
            while (true)
            {
                Console.WriteLine($"第 {Day} 天");
                List<CV_Info> CVInfosTemp = new List<CV_Info>();
                DayActivity.GenerateCVs(ref CVNumber, ref CVInfosTemp);
                int todayCVNumber = CVInfosTemp.Count;

                foreach (var cv in CVInfosTemp)
                {
                    HR.Hire(cv, ref CVInfos);
                }

                foreach (var cv in CVInfos)
                {

                }

                Console.WriteLine($"今日共收到 {todayCVNumber} 份简历：");
                Console.WriteLine("目前员工有：");
                foreach (var staff in CVInfos)
                {
                    Console.WriteLine(staff.Name);
                    Console.WriteLine($"能力1:{(Abilities)staff.selfAbility1},能力2:{(Abilities)staff.selfAbility2}");
                    Console.WriteLine($"工作:{(Works)staff.selfWork},工作收入:{staff.WorkIncome} 牙币,负能量收入:{staff.NegativeEnergyIncome} 点");
                }



                foreach (var cv in CVInfos)
                {
                    if (!cv.IsWorking) continue;
                    Economy.YaFromWork(cv.WorkIncome);
                    NegativeEnergy.BadEnergyFromWork(cv.NegativeEnergyIncome);
                }

                Economy.DailyYaExpence(CVNumber);

                Console.WriteLine($"牙币总量{Economy.AllYa},负能量总量{NegativeEnergy.AllBadEnergy}");
                if (Economy.IsBankrupt || NegativeEnergy.IsOverload)
                {
                    Console.WriteLine("Game Over");
                    break;
                }
                Day++;
            }
        }
    }

    public static class DayActivity
    {
        public static void GenerateCVs(ref int CVNumber, ref List<CV_Info> CVInfosTemp)
        {
            Random CVRandom = new Random();
            int todayCVNumber = CVRandom.Next(1, 3);
            CVNumber += todayCVNumber;
            for (int i = 0; i < todayCVNumber; i++)       //生成当天随机简历
            {
                CV_Info newCV = HR.CV_maker();
                CVInfosTemp.Add(newCV);
            }
        }
    }

    public class HR   //人事部
    {
        public static CV_Info CV_maker()         //生成随机简历
        {
            string Name = Names.GetRandomName();

            Random random = new Random();
            int random1Number = random.Next(Enum.GetNames(typeof(Abilities)).Length);
            int random2Number = random.Next(Enum.GetNames(typeof(Abilities)).Length);
            while (random1Number == random2Number)
            {
                random2Number = random.Next(Enum.GetNames(typeof(Abilities)).Length);
            }
            int selfAbility1 = random1Number * 2;
            int selfAbility2 = random2Number * 2;

            int selfWork = random.Next(Enum.GetNames(typeof(Works)).Length);

            var workResults = DoWork(selfWork, selfAbility1, selfAbility2);

            int WorkIncome = workResults[0];
            int NegativeEnergyIncome = workResults[1];

            return new CV_Info
            {
                Name = Name,
                selfAbility1 = selfAbility1,
                selfAbility2 = selfAbility2,
                selfWork = selfWork,
                WorkIncome = WorkIncome,
                NegativeEnergyIncome = NegativeEnergyIncome,
                IsWorking = true,
            };
        }
        public static void Hire(CV_Info cv, ref List<CV_Info> CVInfos)  //录用方法
        {
            CVInfos.Add(cv);
        }

        public static void Fire(string name, ref List<CV_Info> CVInfos)  //解雇方法
        {
            int index = 0;
            CV_Info FireCV = new CV_Info();
            foreach (var cv in CVInfos)
            {
                if (cv.Name == name)
                {
                    FireCV = cv;
                }
                index++;
            }
            FireCV.IsWorking = false;
            CVInfos[index] = FireCV;
        }

        public static List<int> DoWork(int work, int ability1, int ability2)  //工作方法
        {
            int efficiency;
            int yaIncome;
            int energyIncome;
            if (ability1 == work || ability2 == work)
            {
                efficiency = 2;
                yaIncome = Economy.IncomeOfWork(work, efficiency);
                energyIncome = 0;
            }
            else if (ability1 + 1 == work || ability2 + 1 == work)
            {
                efficiency = 1;
                yaIncome = Economy.IncomeOfWork(work, efficiency);
                energyIncome = 10;

            }
            else
            {
                efficiency = 0;
                yaIncome = Economy.IncomeOfWork(work, efficiency);
                energyIncome = 20;
            }
            return new List<int> { yaIncome, energyIncome };
        }
    }

    public static class Names  //姓名类
    {
        private static string name = File.ReadAllText("HR-Economist//HR&Economist//Names.txt");
        public static string[] names = name.Split('、');
        public static string GetRandomName()   //生成随机简历姓名
        {
            var namesArray = Names.names;
            Random NameRandom = new Random();
            int index = NameRandom.Next(namesArray.Length - 1);
            return namesArray[index];
        }
    }

    public static class Economy //经济结构
    {
        public static int AllYa { get; set; } = 30;  //初始牙币
        public static bool IsBankrupt  //破产判断
        {
            get
            {
                return AllYa <= 0;
            }
        }
        public static int YaFromWork(int workIncome)  //牙币总收入
        {
            return AllYa += workIncome;
        }
        public static int DailyYaExpence(int CVNumber)   //每日牙币支出
        {
            return AllYa -= 10 * CVNumber;
        }
        public static int IncomeOfWork(int work, int efficiency)
        {
            if (efficiency == 2)
            {
                switch (work)
                {
                    case (int)Works.FoodGetter:
                        return 15;
                    case (int)Works.Producer:
                        return 14;
                    case (int)Works.House:
                        return 14;
                    case (int)Works.Builder:
                        return 12;
                    case (int)Works.Craft:
                        return 14;
                    case (int)Works.Manufacturer:
                        return 13;
                    case (int)Works.Medical:
                        return 16;
                    case (int)Works.Medicine:
                        return 12;
                    case (int)Works.Defense:
                        return 16;
                    case (int)Works.Security:
                        return 14;
                    case (int)Works.Explore:
                        return 16;
                    case (int)Works.Developer:
                        return 14;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else if (efficiency == 1)
            {
                switch (work)
                {
                    case (int)Works.FoodGetter:
                        return 11;
                    case (int)Works.Producer:
                        return 9;
                    case (int)Works.House:
                        return 11;
                    case (int)Works.Builder:
                        return 9;
                    case (int)Works.Craft:
                        return 10;
                    case (int)Works.Manufacturer:
                        return 10;
                    case (int)Works.Medical:
                        return 12;
                    case (int)Works.Medicine:
                        return 9;
                    case (int)Works.Defense:
                        return 11;
                    case (int)Works.Security:
                        return 11;
                    case (int)Works.Explore:
                        return 12;
                    case (int)Works.Developer:
                        return 10;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                switch (work)
                {
                    case (int)Works.FoodGetter:
                        return 7;
                    case (int)Works.Producer:
                        return 6;
                    case (int)Works.House:
                        return 7;
                    case (int)Works.Builder:
                        return 6;
                    case (int)Works.Craft:
                        return 7;
                    case (int)Works.Manufacturer:
                        return 6;
                    case (int)Works.Medical:
                        return 8;
                    case (int)Works.Medicine:
                        return 7;
                    case (int)Works.Defense:
                        return 7;
                    case (int)Works.Security:
                        return 7;
                    case (int)Works.Explore:
                        return 8;
                    case (int)Works.Developer:
                        return 7;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }

    public static class NegativeEnergy    //负能量结构
    {
        public static int AllBadEnergy { get; set; } = 0;
        public static bool IsOverload
        {
            get
            {
                return AllBadEnergy >= 2400;
            }
        }
        public static int BadEnergyFromWork(int negativeEnergyIncome)
        {
            return AllBadEnergy += negativeEnergyIncome;
        }
    }
}
