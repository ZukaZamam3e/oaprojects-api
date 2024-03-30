using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAProjects.Data.OAIdentity.Entities;
public class OA_USER_TOKEN
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int USER_TOKEN_ID { get; set; }

    public int USER_ID { get; set; }

    public string TOKEN { get; set; }

    public int EXPIRY_TIME { get; set; }

    public int ISSUED_AT { get; set; }

    public DateTime EXPIRY_DATE_UTC { get; set; }

    public DateTime ISSUED_AT_DATE_UTC { get; set; }
}
