//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
#pragma warning disable 1591

// Option: missing-value detection (*Specified/ShouldSerialize*/Reset*) enabled
    
// Generated from: steammessages_secrets.steamclient.proto
// Note: requires additional types generated from: steammessages_unified_base.steamclient.proto
namespace SteamKit2.Unified.Internal
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"CKeyEscrow_Request")]
  public partial class CKeyEscrow_Request : global::ProtoBuf.IExtensible
  {
    public CKeyEscrow_Request() {}
    

    private byte[] _rsa_oaep_sha_ticket;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"rsa_oaep_sha_ticket", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public byte[] rsa_oaep_sha_ticket
    {
      get { return _rsa_oaep_sha_ticket?? null; }
      set { _rsa_oaep_sha_ticket = value; }
    }
    [global::System.Xml.Serialization.XmlIgnore]
    [global::System.ComponentModel.Browsable(false)]
    public bool rsa_oaep_sha_ticketSpecified
    {
      get { return _rsa_oaep_sha_ticket != null; }
      set { if (value == (_rsa_oaep_sha_ticket== null)) _rsa_oaep_sha_ticket = value ? this.rsa_oaep_sha_ticket : (byte[])null; }
    }
    private bool ShouldSerializersa_oaep_sha_ticket() { return rsa_oaep_sha_ticketSpecified; }
    private void Resetrsa_oaep_sha_ticket() { rsa_oaep_sha_ticketSpecified = false; }
    

    private byte[] _password;
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"password", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public byte[] password
    {
      get { return _password?? null; }
      set { _password = value; }
    }
    [global::System.Xml.Serialization.XmlIgnore]
    [global::System.ComponentModel.Browsable(false)]
    public bool passwordSpecified
    {
      get { return _password != null; }
      set { if (value == (_password== null)) _password = value ? this.password : (byte[])null; }
    }
    private bool ShouldSerializepassword() { return passwordSpecified; }
    private void Resetpassword() { passwordSpecified = false; }
    

    private EKeyEscrowUsage? _usage;
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"usage", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public EKeyEscrowUsage usage
    {
      get { return _usage?? EKeyEscrowUsage.k_EKeyEscrowUsageStreamingDevice; }
      set { _usage = value; }
    }
    [global::System.Xml.Serialization.XmlIgnore]
    [global::System.ComponentModel.Browsable(false)]
    public bool usageSpecified
    {
      get { return _usage != null; }
      set { if (value == (_usage== null)) _usage = value ? this.usage : (EKeyEscrowUsage?)null; }
    }
    private bool ShouldSerializeusage() { return usageSpecified; }
    private void Resetusage() { usageSpecified = false; }
    

    private string _device_name;
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"device_name", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string device_name
    {
      get { return _device_name?? ""; }
      set { _device_name = value; }
    }
    [global::System.Xml.Serialization.XmlIgnore]
    [global::System.ComponentModel.Browsable(false)]
    public bool device_nameSpecified
    {
      get { return _device_name != null; }
      set { if (value == (_device_name== null)) _device_name = value ? this.device_name : (string)null; }
    }
    private bool ShouldSerializedevice_name() { return device_nameSpecified; }
    private void Resetdevice_name() { device_nameSpecified = false; }
    
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"CKeyEscrow_Ticket")]
  public partial class CKeyEscrow_Ticket : global::ProtoBuf.IExtensible
  {
    public CKeyEscrow_Ticket() {}
    

    private byte[] _password;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"password", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public byte[] password
    {
      get { return _password?? null; }
      set { _password = value; }
    }
    [global::System.Xml.Serialization.XmlIgnore]
    [global::System.ComponentModel.Browsable(false)]
    public bool passwordSpecified
    {
      get { return _password != null; }
      set { if (value == (_password== null)) _password = value ? this.password : (byte[])null; }
    }
    private bool ShouldSerializepassword() { return passwordSpecified; }
    private void Resetpassword() { passwordSpecified = false; }
    

    private ulong? _identifier;
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"identifier", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public ulong identifier
    {
      get { return _identifier?? default(ulong); }
      set { _identifier = value; }
    }
    [global::System.Xml.Serialization.XmlIgnore]
    [global::System.ComponentModel.Browsable(false)]
    public bool identifierSpecified
    {
      get { return _identifier != null; }
      set { if (value == (_identifier== null)) _identifier = value ? this.identifier : (ulong?)null; }
    }
    private bool ShouldSerializeidentifier() { return identifierSpecified; }
    private void Resetidentifier() { identifierSpecified = false; }
    

    private byte[] _payload;
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"payload", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public byte[] payload
    {
      get { return _payload?? null; }
      set { _payload = value; }
    }
    [global::System.Xml.Serialization.XmlIgnore]
    [global::System.ComponentModel.Browsable(false)]
    public bool payloadSpecified
    {
      get { return _payload != null; }
      set { if (value == (_payload== null)) _payload = value ? this.payload : (byte[])null; }
    }
    private bool ShouldSerializepayload() { return payloadSpecified; }
    private void Resetpayload() { payloadSpecified = false; }
    

    private uint? _timestamp;
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"timestamp", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public uint timestamp
    {
      get { return _timestamp?? default(uint); }
      set { _timestamp = value; }
    }
    [global::System.Xml.Serialization.XmlIgnore]
    [global::System.ComponentModel.Browsable(false)]
    public bool timestampSpecified
    {
      get { return _timestamp != null; }
      set { if (value == (_timestamp== null)) _timestamp = value ? this.timestamp : (uint?)null; }
    }
    private bool ShouldSerializetimestamp() { return timestampSpecified; }
    private void Resettimestamp() { timestampSpecified = false; }
    

    private EKeyEscrowUsage? _usage;
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"usage", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public EKeyEscrowUsage usage
    {
      get { return _usage?? EKeyEscrowUsage.k_EKeyEscrowUsageStreamingDevice; }
      set { _usage = value; }
    }
    [global::System.Xml.Serialization.XmlIgnore]
    [global::System.ComponentModel.Browsable(false)]
    public bool usageSpecified
    {
      get { return _usage != null; }
      set { if (value == (_usage== null)) _usage = value ? this.usage : (EKeyEscrowUsage?)null; }
    }
    private bool ShouldSerializeusage() { return usageSpecified; }
    private void Resetusage() { usageSpecified = false; }
    

    private string _device_name;
    [global::ProtoBuf.ProtoMember(6, IsRequired = false, Name=@"device_name", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string device_name
    {
      get { return _device_name?? ""; }
      set { _device_name = value; }
    }
    [global::System.Xml.Serialization.XmlIgnore]
    [global::System.ComponentModel.Browsable(false)]
    public bool device_nameSpecified
    {
      get { return _device_name != null; }
      set { if (value == (_device_name== null)) _device_name = value ? this.device_name : (string)null; }
    }
    private bool ShouldSerializedevice_name() { return device_nameSpecified; }
    private void Resetdevice_name() { device_nameSpecified = false; }
    

    private string _device_model;
    [global::ProtoBuf.ProtoMember(7, IsRequired = false, Name=@"device_model", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string device_model
    {
      get { return _device_model?? ""; }
      set { _device_model = value; }
    }
    [global::System.Xml.Serialization.XmlIgnore]
    [global::System.ComponentModel.Browsable(false)]
    public bool device_modelSpecified
    {
      get { return _device_model != null; }
      set { if (value == (_device_model== null)) _device_model = value ? this.device_model : (string)null; }
    }
    private bool ShouldSerializedevice_model() { return device_modelSpecified; }
    private void Resetdevice_model() { device_modelSpecified = false; }
    

    private string _device_serial;
    [global::ProtoBuf.ProtoMember(8, IsRequired = false, Name=@"device_serial", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string device_serial
    {
      get { return _device_serial?? ""; }
      set { _device_serial = value; }
    }
    [global::System.Xml.Serialization.XmlIgnore]
    [global::System.ComponentModel.Browsable(false)]
    public bool device_serialSpecified
    {
      get { return _device_serial != null; }
      set { if (value == (_device_serial== null)) _device_serial = value ? this.device_serial : (string)null; }
    }
    private bool ShouldSerializedevice_serial() { return device_serialSpecified; }
    private void Resetdevice_serial() { device_serialSpecified = false; }
    

    private uint? _device_provisioning_id;
    [global::ProtoBuf.ProtoMember(9, IsRequired = false, Name=@"device_provisioning_id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public uint device_provisioning_id
    {
      get { return _device_provisioning_id?? default(uint); }
      set { _device_provisioning_id = value; }
    }
    [global::System.Xml.Serialization.XmlIgnore]
    [global::System.ComponentModel.Browsable(false)]
    public bool device_provisioning_idSpecified
    {
      get { return _device_provisioning_id != null; }
      set { if (value == (_device_provisioning_id== null)) _device_provisioning_id = value ? this.device_provisioning_id : (uint?)null; }
    }
    private bool ShouldSerializedevice_provisioning_id() { return device_provisioning_idSpecified; }
    private void Resetdevice_provisioning_id() { device_provisioning_idSpecified = false; }
    
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"CKeyEscrow_Response")]
  public partial class CKeyEscrow_Response : global::ProtoBuf.IExtensible
  {
    public CKeyEscrow_Response() {}
    

    private CKeyEscrow_Ticket _ticket = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"ticket", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public CKeyEscrow_Ticket ticket
    {
      get { return _ticket; }
      set { _ticket = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
    [global::ProtoBuf.ProtoContract(Name=@"EKeyEscrowUsage", EnumPassthru=true)]
    public enum EKeyEscrowUsage
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"k_EKeyEscrowUsageStreamingDevice", Value=0)]
      k_EKeyEscrowUsageStreamingDevice = 0
    }
  
    public interface ISecrets
    {
      CKeyEscrow_Response KeyEscrow(CKeyEscrow_Request request);
    
    }
    
    
}
#pragma warning restore 1591