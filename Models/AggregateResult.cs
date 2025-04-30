namespace AggregatorAPI.Models{


    public class Aggr<T> where T : AggregateResult{
        public List<T> ResList{get;set;} = new List<T>();

        public Aggr(params T[] values){

            foreach(var v in values)
                ResList.Add(v);
        }
    }
    public class AggregateResult{
        /// <summary>
        /// The api that have been called
        /// </summary>
        public required string ApiUrl{get;set;}

        /// <summary>
        /// The result of the call . If the repsonse is ok return the object else the error message
        /// </summary>
        public required string Result{get;set;}
    }
}