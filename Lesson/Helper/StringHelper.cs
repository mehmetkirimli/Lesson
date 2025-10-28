namespace Lesson.Helper
{
    // Lesson: Static Class (Statik Sınıf)
    // 'static' sınıflar 'new' ile oluşturulamazlar (örneği alınamaz).
    // Tüm metotları ve property'leri de 'static' olmalıdır.

    // static -> Nesneye değil , doğrudan sınıfa ait.
    // Genellikle 'Helper' veya 'Utility' (yardımcı) metotları toplamak için kullanılır.
    // Örn: Math.Pow(), String.IsNullOrEmpty()
    public static class StringHelper
    {
        public static string Truncate(string value, int length) 
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= length ? value : value.Substring(0, length);
        }
    }
}
