namespace HR_Economist
{
    public struct CV_Info  //简历信息
    {
        public string Name;  //姓名
        public int selfAbility1;  //能力值1(每人两种能力)
        public int selfAbility2;  //能力值2(每人两种能力)
    }
    public enum Abilities   //能力种类
    {
    /*
    1 食物获取与生产
    2 居所建造
    3 工艺与制造
    4 医疗
    5 防御与安全
    6 探索与开拓
    7 精神文化与信仰
    */
    FoodGetter_Producer,
    House_Builder,
    Craft_Manufacturer,
    Medical,
    Defense_Security,
    Explore_Developer,
    Spirit_Culture_Faith 
    };
    internal class Program
    {
        static void Main(string[] args)
        {
            int Day = 1;
            List<CV_Info> CVInfos = new List<CV_Info>();
            while (true)
            {
                Console.WriteLine($"第 {Day} 天");
                Random CVRandom = new Random();
                int CVNumber = CVRandom.Next(1, 3);
                List<CV_Info> CVInfosTemp = new List<CV_Info>();
                for (int i = 0; i < CVNumber; i++)                              //生成随机简历
                {
                    CV_Info newCV = CVFactory.CV_maker();
                    CVInfosTemp.Add(newCV);
                }

                CVInfos.AddRange(CVInfosTemp);
                
                Console.WriteLine($"今日共收到 {CVNumber} 份简历：");
                Console.WriteLine("目前员工有：");    
                foreach (var staff in CVInfos)
                {
                    Console.WriteLine(staff.Name);
                    Console.WriteLine($"能力1:{(Abilities)staff.selfAbility1},能力2:{(Abilities)staff.selfAbility2}");
                }
                if (Day == 2)
                {
                    Console.WriteLine("Game Over");
                }
                Day++;
            }
        }

    }
    
    public class CVFactory   //简历工厂
    {
        public static CV_Info CV_maker()
        {
            Random random1 = new Random();
            Random random2 = new Random();
            return new CV_Info                     //生成随机简历
            {
                Name = Names.GetRandomName(),
                selfAbility1 = random1.Next(Enum.GetNames(typeof(Abilities)).Length - 1),
                selfAbility2 = random2.Next(Enum.GetNames(typeof(Abilities)).Length - 1) 
            };
        }
    }

    public static class Names  //姓名类
    {
        private static string name = File.ReadAllText("Names.txt");
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
