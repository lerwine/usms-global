using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Nodes;

namespace SnTsTypeGenerator.Models;

/// <summary>
/// Represents an item from the "Dictionary Entry" (<see cref="SnApiConstants.TABLE_NAME_SYS_DICTIONARY" />) table.
/// </summary>
[Table(nameof(Services.TypingsDbContext.Elements))]
public class Element : IEquatable<Element>
{
    private readonly object _syncRoot = new();

    private string _name = string.Empty;

    /// <summary>
    /// Value of the "Column name" (<see cref="SnApiConstants.JSON_KEY_ELEMENT" />) column.
    /// </summary>
    [Key]
    [BackingField(nameof(_name))]
    public string Name
    {
        get => _name;
        set => _name = value ?? string.Empty;
    }

    private string _label = string.Empty;

    /// <summary>
    /// Value of the "Column label" (<see cref="SnApiConstants.JSON_KEY_COLUMN_LABEL" />) column.
    /// </summary>
    [NotNull]
    [BackingField(nameof(_label))]
    public string Label
    {
        get => _label;
        set => _label = value ?? string.Empty;
    }

    /// <summary>
    /// Value of the "Active" (<see cref="SnApiConstants.JSON_KEY_ACTIVE" />) column.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Value of the "Array" (<see cref="SnApiConstants.JSON_KEY_ARRAY" />) column.
    /// </summary>
    public bool IsArray { get; set; }

    /// <summary>
    /// Value of the "Max length" (<see cref="SnApiConstants.JSON_KEY_MAX_LENGTH" />) column.
    /// </summary>
    public int? MaxLength { get; set; }

    /// <summary>
    /// Value of the "Comments" (<see cref="SnApiConstants.JSON_KEY_COMMENTS" />) column.
    /// </summary>
    public string? Comments { get; set; }

    /// <summary>
    /// Value of the "Default value" (<see cref="SnApiConstants.JSON_KEY_DEFAULT_VALUE" />) column.
    /// </summary>
    public string? DefaultValue { get; set; }

    /// <summary>
    /// Value of the "Display" (<see cref="SnApiConstants.JSON_KEY_DISPLAY" />) column.
    /// </summary>
    public bool IsDisplay { get; set; }

    /// <summary>
    /// Value of the "Sizeclass" (<see cref="SnApiConstants.JSON_KEY_SIZECLASS" />) column.
    /// </summary>
    public int? SizeClass { get; set; }

    /// <summary>
    /// Value of the "Mandatory" (<see cref="SnApiConstants.JSON_KEY_MANDATORY" />) column.
    /// </summary>
    public bool IsMandatory { get; set; }

    /// <summary>
    /// Value of the "Primary" (<see cref="SnApiConstants.JSON_KEY_PRIMARY" />) column.
    /// </summary>
    public bool IsPrimary { get; set; }

    /// <summary>
    /// Value of the "Read only" (<see cref="SnApiConstants.JSON_KEY_READ_ONLY" />) column.
    /// </summary>
    public bool IsReadOnly { get; set; }

    /// <summary>
    /// Value of the "Calculated" (<see cref="SnApiConstants.JSON_KEY_VIRTUAL" />) column.
    /// </summary>
    public bool IsCalculated { get; set; }

    /// <summary>
    /// Value of the "Unique" (<see cref="SnApiConstants.JSON_KEY_UNIQUE" />) column.
    /// </summary>
    public bool IsUnique { get; set; }

    /// <summary>
    /// Date and time that this record was last updated.
    /// </summary>
    public DateTime LastUpdated { get; set; }

    private string? _packageName;

    /// <summary>
    /// Name of the associated record for the "Package" (<see cref="SnApiConstants.JSON_KEY_SYS_PACKAGE" />) column.
    /// </summary>
    [BackingField(nameof(_packageName))]
    public string? PackageName
    {
        get => _package?.Name ?? _packageName;
        set
        {
            lock (_syncRoot)
            {
                if (value is null)
                {
                    if (_packageName is not null)
                    {
                        _packageName = null;
                        _package = null;
                    }
                }
                else if (_packageName is null || !value.Equals(_packageName, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (_package is null)
                        _packageName = value;
                    else if (value.Equals(_package.Name, StringComparison.InvariantCultureIgnoreCase))
                        _packageName = null;
                    else
                        _package = null;
                }
            }
        }
    }

    private Package? _package;

    /// <summary>
    /// The source package for the element.
    /// </summary>
    public Package? Package
    {
        get => _package;
        set
        {
            lock (_syncRoot)
            {
                if ((value is null) ? _package is null : _package is not null && ReferenceEquals(_package, value))
                    return;

                _package = value;
                _packageName = null;
            }
        }
    }

    private string _tableName = string.Empty;

    /// <summary>
    /// Value of the "Table" (<see cref="SnApiConstants.JSON_KEY_NAME" />) column.
    /// </summary>
    [BackingField(nameof(_tableName))]
    [Key]
    public string TableName
    {
        get => _table?.Name ?? _tableName;
        set
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));
            lock (_syncRoot)
            {
                if (_table is null || !value.Equals(_table.Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    _tableName = value;
                    _table = null;
                }
            }
        }
    }

    private Table? _table;

    /// <summary>
    /// The table that the current element belongs to.
    /// </summary>
    public Table? Table
    {
        get => _table;
        set
        {
            lock (_syncRoot)
            {
                if (value is null)
                {
                    if (_table is null)
                        return;
                    _tableName = _table.Name;
                }
                else
                {
                    if (_table is not null && ReferenceEquals(_table, value))
                        return;
                    _table = value;
                    _tableName = string.Empty;
                }
            }
        }
    }

    private string _typeName = string.Empty;

    /// <summary>
    /// Name of the related value for the "Type" (<see cref="SnApiConstants.JSON_KEY_INTERNAL_TYPE" />) column.
    /// </summary>
    [BackingField(nameof(_typeName))]
    public string TypeName
    {
        get => _type?.Name ?? _typeName;
        set
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));
            lock (_syncRoot)
            {
                if (_type is null || !value.Equals(_type.Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    _typeName = value;
                    _type = null;
                }
            }
        }
    }

    private GlideType? _type;

    /// <summary>
    /// The record representing the column type.
    /// </summary>
    public GlideType? Type
    {
        get => _type;
        set
        {
            lock (_syncRoot)
            {
                if (value is null)
                {
                    if (_type is null)
                        return;
                    _typeName = _type.Name;
                }
                else
                {
                    if (_type is not null && ReferenceEquals(_type, value))
                        return;
                    _type = value;
                    _typeName = string.Empty;
                }
            }
        }
    }

    private string? _refTableName;

    /// <summary>
    /// The value of the associated name for the "Reference" (<see cref="SnApiConstants.JSON_KEY_REFERENCE" />) column.
    /// </summary>
    [BackingField(nameof(_refTableName))]
    public string? RefTableName
    {
        get => _reference?.Name ?? _refTableName;
        set
        {
            lock (_syncRoot)
            {
                if (value is null)
                {
                    if (_refTableName is not null)
                    {
                        _refTableName = null;
                        _reference = null;
                    }
                }
                else if (_refTableName is null || !value.Equals(_refTableName, StringComparison.InvariantCultureIgnoreCase))
                {

                    if (_reference is null)
                        _refTableName = value;
                    else if (value.Equals(_reference.Name, StringComparison.InvariantCultureIgnoreCase))
                        _refTableName = null;
                    else
                        _reference = null;
                }
            }
        }
    }

    private Table? _reference;

    /// <summary>
    /// The table the current column refers to.
    /// </summary>
    public Table? Reference
    {
        get => _reference;
        set
        {
            lock (_syncRoot)
            {
                if ((value is null) ? _reference is null : _reference is not null && ReferenceEquals(_reference, value))
                    return;

                _reference = value;
                _refTableName = null;
            }
        }
    }

    private string _sourceFqdn = string.Empty;

    /// <summary>
    /// The FQDN of the source ServiceNow instance.
    /// </summary>
    [BackingField(nameof(_sourceFqdn))]
    public string SourceFqdn
    {
        get => _source?.FQDN ?? _sourceFqdn;
        set
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));
            lock (_syncRoot)
            {
                if (_source is null || !value.Equals(_source.FQDN, StringComparison.InvariantCultureIgnoreCase))
                {
                    _sourceFqdn = value;
                    _source = null;
                }
            }
        }
    }

    private SncSource? _source;

    /// <summary>
    /// The record representing the source ServiceNow instance.
    /// </summary>
    public SncSource? Source
    {
        get => _source;
        set
        {
            lock (_syncRoot)
            {
                if (value is null)
                {
                    if (_source is null)
                        return;
                    _sourceFqdn = _source.FQDN;
                }
                else
                {
                    if (_source is not null && ReferenceEquals(_source, value))
                        return;
                    _source = value;
                    _sourceFqdn = string.Empty;
                }
            }
        }
    }

    private string _sysID = string.Empty;

    /// <summary>
    /// Value of the "Sys ID" (<see cref="SnApiConstants.JSON_KEY_SYS_ID" />) column.
    /// </summary>
    [NotNull]
    [BackingField(nameof(_sysID))]
    public string SysID
    {
        get => _sysID;
        set => _sysID = value ?? string.Empty;
    }

    public bool Equals(Element? other) => other is not null && (ReferenceEquals(this, other) ||
        (Services.SnApiConstants.NameComparer.Equals(_name, other._name) && Services.SnApiConstants.NameComparer.Equals(_tableName, other._tableName)));

    public override bool Equals(object? obj) => Equals(obj as Element);

    public override int GetHashCode()
    {
        unchecked
        {
            return (21 + Services.SnApiConstants.NameComparer.GetHashCode(_name)) * 7 + (_table?.GetHashCode() ?? Services.SnApiConstants.NameComparer.GetHashCode(_tableName));
        }
    }

    public override string ToString() => nameof(Element) + new JsonObject()
    {
        { nameof(Name), JsonValue.Create(_name) },
        { nameof(Label), JsonValue.Create(_label) },
        { nameof(IsActive), JsonValue.Create(IsActive) },
        { nameof(IsArray), JsonValue.Create(IsArray) },
        { nameof(MaxLength), JsonValue.Create(MaxLength) },
        { nameof(Comments), JsonValue.Create(Comments) },
        { nameof(DefaultValue), JsonValue.Create(DefaultValue) },
        { nameof(IsDisplay), JsonValue.Create(IsDisplay) },
        { nameof(SizeClass), JsonValue.Create(SizeClass) },
        { nameof(IsMandatory), JsonValue.Create(IsMandatory) },
        { nameof(IsPrimary), JsonValue.Create(IsPrimary) },
        { nameof(IsReadOnly), JsonValue.Create(IsReadOnly) },
        { nameof(IsCalculated), JsonValue.Create(IsCalculated) },
        { nameof(IsUnique), JsonValue.Create(IsUnique) },
        { nameof(LastUpdated), JsonValue.Create(LastUpdated) },
        { nameof(Package), JsonValue.Create(_packageName) },
        { nameof(Table), JsonValue.Create(_tableName) },
        { nameof(Type), JsonValue.Create(_typeName) },
        { nameof(Reference), JsonValue.Create(_refTableName) },
        { nameof(Source), JsonValue.Create(_sourceFqdn) },
        { nameof(SysID), JsonValue.Create(_sysID) }
    }.ToJsonString();
}