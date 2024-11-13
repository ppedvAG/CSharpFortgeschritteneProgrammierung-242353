namespace LinqSamples.Extensions
{
    internal static class ExtensionMethods
    {
        public static int DigitSum(this int number)
        {
            // Ein String ist ein character Array weswegen wir hier Linq benutzen können
            return number.ToString().Sum(e => (int)char.GetNumericValue(e));
        }
    }
}
