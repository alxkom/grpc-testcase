namespace TaskManager.Models;
using System.Runtime.Serialization;

[DataContract]
public class Task
{
    [DataMember(Order = 1)]
    public int Id { get; set; }

    [DataMember(Order = 2)]
    public required string Name { get; set; }

    [DataMember(Order = 3)]
    public int UserId { get; set; }
    
    [DataMember(Order = 4)]
    public TaskState State { get; set; }

    // Navigation property
    [DataMember(Order = 5)]
    public User? User { get; set; }
}