using Demo;
using Google.Protobuf;
using UnityEngine;

public class ProtobufTest : MonoBehaviour
{
    private void Start()
    {
        var original = new Person
        {
            Id = 123,
            Name = "张三"
        };

        byte[] bytes = original.ToByteArray();
        Person restored = Person.Parser.ParseFrom(bytes);

        if (!original.Equals(restored))
            throw new System.Exception("Protobuf 验证失败");

        Debug.Log($"Id={restored.Id}, Name={restored.Name}");
        Debug.Log($"二进制：{System.BitConverter.ToString(bytes)}");
        Debug.Log("Protobuf 验证通过");
    }
}