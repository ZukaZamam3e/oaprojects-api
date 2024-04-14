using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAProjects.Data.OAIdentity.Entities;

public enum OATableIds
{
    OA_USER = 1,
    OA_USER_TOKEN
}

public class OA_ID_XREF
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ID_XREF_ID { get; set; }

    public int TABLE_ID { get; set; }

    public int OLD_ID { get; set; }

    public int NEW_ID { get; set; }
}
