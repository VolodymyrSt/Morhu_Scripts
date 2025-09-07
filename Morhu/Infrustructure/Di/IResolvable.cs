namespace Di
{
    public interface IResolvable
    {
        public T Resolve<T>(string tag = null);
    }
}
