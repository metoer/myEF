using DataBase.Entity.ModelBase;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Models
{
    public class mc_medialog
    {
        [Key]
        public string record_id { get; set; }
        public string user_guid { get; set; }
        public string user_id { get; set; }
        public string user_name { get; set; }
        public string user_code { get; set; }
        public string org_id { get; set; }
        public string org_id_code { get; set; }
        public string org_name { get; set; }
        public string device_id { get; set; }
        public string device_code { get; set; }
        public string record_time { get; set; }
        public string collect_time { get; set; }
        public int collect_process { get; set; }
        public string collect_ip { get; set; }
        public int media_type { get; set; }
        public string file_code { get; set; }
        public string file_path { get; set; }
        public string file_name { get; set; }
        public string file_data { get; set; }
        public long file_size { get; set; }
        public int duration { get; set; }
        public int resolution_width { get; set; }
        public int resolution_height { get; set; }
        public int encode_type { get; set; }
        public int encrypt_type { get; set; }
        public int arith_type { get; set; }
        public int camera_imp { get; set; }
        public string camera_tag { get; set; }
        public int user_imp { get; set; }
        public string user_tag { get; set; }
        public string storage_server { get; set; }
        public string play_location { get; set; }
        public string storage_location { get; set; }
        public string station_code { get; set; }
        public string label_type { get; set; }
        public string file_type { get; set; }
        public string thumbnail { get; set; }
        public int upload_process { get; set; }
     
    }

    public class ds_medialog
    {
        [Key]
        public string record_id { get; set; }
        public string user_guid { get; set; }
        public string user_id { get; set; }
        public string user_name { get; set; }
        public string user_code { get; set; }
        public string org_id { get; set; }
        public string org_id_code { get; set; }
        public string org_name { get; set; }
        public string device_id { get; set; }
        public string device_code { get; set; }
        public string record_time { get; set; }
        public string collect_time { get; set; }
        public int collect_process { get; set; }
        public string collect_ip { get; set; }
        public int media_type { get; set; }
        public string file_code { get; set; }
        public string file_path { get; set; }
        public string file_name { get; set; }
        public string file_data { get; set; }
        public long file_size { get; set; }
        public int duration { get; set; }
        public int resolution_width { get; set; }
        public int resolution_height { get; set; }
        public int encode_type { get; set; }
        public int encrypt_type { get; set; }
        public int arith_type { get; set; }
        public int camera_imp { get; set; }
        public string camera_tag { get; set; }
        public int user_imp { get; set; }
        public string user_tag { get; set; }
        public string storage_server { get; set; }
        public string play_location { get; set; }
        public string storage_location { get; set; }
        public string station_code { get; set; }
        public string label_type { get; set; }
        public string file_type { get; set; }
        public string thumbnail { get; set; }
        public int upload_process { get; set; }
     
        public int file_upload_process { get; set; }
    }

    [Export("ds_medialogMapping")]
    public class mappingds_medialog : MappingBase<ds_medialog>
    { }

    [Export("mc_medialogMapping")]
    public class mappingmc_medialog : MappingBase<mc_medialog>
    { }
}
