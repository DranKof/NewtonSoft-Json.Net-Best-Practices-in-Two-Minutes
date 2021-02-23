Imports System.Reflection

Class PrivateSetResolver
    Inherits DefaultContractResolver

    Protected Overrides Function CreateProperty(member As MemberInfo, serialization As MemberSerialization) As JsonProperty
        Dim jsonProperty As JsonProperty = MyBase.CreateProperty(member, serialization)
        If Not jsonProperty.Writable AndAlso member.GetType Is GetType(PropertyInfo) Then
            Dim pInfo As PropertyInfo = CType(member, PropertyInfo)
            jsonProperty.Writable = pInfo.GetSetMethod(True) <> Nothing
        End If
        Return jsonProperty
    End Function

End Class
