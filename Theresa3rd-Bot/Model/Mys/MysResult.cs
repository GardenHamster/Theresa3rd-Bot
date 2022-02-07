namespace Theresa3rd_Bot.Model.Mys
{
    public class MysResult<T>
    {
        public int retcode { get; set; }
        public string message { get; set; }
        public T data { get; set; }

        public bool isSuccess()
        {
            return retcode == 0;
        }

        public bool isAlreadySign()
        {
            return retcode == -5003 || retcode == 5003 || retcode == -1008 || retcode == 1008;
        }

        public bool isLoginFailure()
        {
            return retcode == -100 || retcode == 100;
        }

    }
}
