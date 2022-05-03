namespace RollCallSystem.Logic
{
    public static class RollCallCodeGenerator
    {
        public static int GenerateCode()
        {
            string code = "";
            string chars = "123456789";
            Random random = new Random();

            for (int i = 0; i < 4; i++)
            {
                code += chars[random.Next(chars.Length)];
            }

            return Convert.ToInt32(code);
        }
    }
}
