namespace TaskManager.Models;
using System.Runtime.Serialization;

[DataContract]
public class User
{
    [DataMember(Order = 1)]
    public int Id { get; set; }

    [DataMember(Order = 2)]
    public required string Name { get; set; }
}