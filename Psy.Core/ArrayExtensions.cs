namespace Psy.Core
{
    public static class ArrayExtensions
    {
        public static T[] Cycle<T>(this T[] array, int direction)
        {
            var result = new T[array.Length];
            for (var i = 0; i < array.Length; i++)
            {
                var source = i + direction;

                while (source > array.Length - 1)
                {
                    source -= array.Length;
                }

                while (source < 0)
                {
                    source += array.Length;
                }

                result[i] = array[source];
            }

            return result;
        }
    }
}