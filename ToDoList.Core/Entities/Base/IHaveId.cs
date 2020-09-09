namespace ToDoList.Core.Entities.Base
{
    public interface IHaveId
    {
        /// <summary>
        /// Identifier
        /// </summary>
        int Id { get; set; }
    }
}