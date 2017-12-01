using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Drone.Entities.Facebook
{
	public class Demographic<T>
	{
		public List<DemographicData<T>> Data { get; set; }
	}

	public class DemographicData<T>
	{
		public string id { get; set; }
		public string Name { get; set; }
		public List<Daily<T>> Days { get; set; }
	}

	public class Daily<T>
	{
		public Gender Gender { get; set; }
		public Country Country { get; set; }
		public Locale Locale { get; set; }
		public string End_Time { get; set; }
	}

	public class Gender
	{
		public int M_25to34 { get; set; }
		public int M_18to24 { get; set; }
		public int M_35to44 { get; set; }
		public int M_45to54 { get; set; }
		public int F_25to34 { get; set; }
		public int F_35to44 { get; set; }
		public int F_45to54 { get; set; }
		public int F_18to24 { get; set; }
		public int M_55to64 { get; set; }
		public int M_13to17 { get; set; }
		public int F_55to64 { get; set; }
		public int M_65to { get; set; }
		public int F_13to17 { get; set; }
		public int F_65to { get; set; }
		public int U_25to34 { get; set; }
		public int U_35to44 { get; set; }
		public int U_45to54 { get; set; }
		public int U_18to24 { get; set; }
		public int U_55to64 { get; set; }
		public int U_65to { get; set; }
	}

	public class Country
	{
		public int US { get; set; }
		public int IN { get; set; }
		public int CA { get; set; }
		public int TH { get; set; }
		public int GB { get; set; }
		public int ID { get; set; }
		public int TR { get; set; }
		public int EG { get; set; }
		public int PH { get; set; }
		public int AU { get; set; }
		public int PK { get; set; }
		public int MX { get; set; }
		public int MY { get; set; }
		public int VN { get; set; }
		public int CO { get; set; }
		public int IT { get; set; }
		public int PE { get; set; }
		public int CN { get; set; }
		public int SA { get; set; }
		public int RO { get; set; }
		public int BD { get; set; }
		public int NG { get; set; }
		public int LK { get; set; }
		public int ES { get; set; }
		public int AE { get; set; }
		public int DE { get; set; }
		public int IL { get; set; }
		public int PT { get; set; }
		public int BR { get; set; }
		public int SG { get; set; }
		public int CR { get; set; }
		public int AR { get; set; }
		public int BG { get; set; }
		public int JO { get; set; }
		public int HK { get; set; }
		public int RS { get; set; }
		public int FR { get; set; }
		public int TW { get; set; }
		public int MA { get; set; }
		public int RU { get; set; }
		public int VE { get; set; }
		public int HU { get; set; }
		public int GR { get; set; }
		public int TN { get; set; }
		public int KR { get; set; }
	}

	public class Locale
	{
		public int en_US { get; set; }
		public int en_GB { get; set; }
		public int zh_CN { get; set; }
		public int es_LA { get; set; }
		public int tr_TR { get; set; }
		public int th_TH { get; set; }
		public int id_ID { get; set; }
		public int ar_AR { get; set; }
		public int fr_FR { get; set; }
		public int es_ES { get; set; }
		public int vi_VN { get; set; }
		public int it_IT { get; set; }
		public int ru_RU { get; set; }
		public int pt_BR { get; set; }
		public int zh_TW { get; set; }
		public int pt_PT { get; set; }
		public int de_DE { get; set; }
		public int bg_BG { get; set; }
		public int zh_HK { get; set; }
		public int ro_RO { get; set; }
		public int fr_CA { get; set; }
		public int hu_HU { get; set; }
		public int ko_KR { get; set; }
		public int pl_PL { get; set; }
		public int nl_NL { get; set; }
		public int he_IL { get; set; }
		public int sq_AL { get; set; }
		public int en_PI { get; set; }
		public int sr_RS { get; set; }
		public int el_GR { get; set; }
		public int sv_SE { get; set; }
		public int hr_HR { get; set; }
		public int en_IN { get; set; }
		public int ja_JP { get; set; }
		public int cs_CZ { get; set; }
		public int fa_IR { get; set; }
		public int nb_NO { get; set; }
		public int ka_GE { get; set; }
		public int da_DK { get; set; }
		public int mk_MK { get; set; }
		public int sk_SK { get; set; }
		public int ms_MY { get; set; }
		public int sl_SI { get; set; }
		public int lt_LT { get; set; }
		public int fi_FI { get; set; }
	}
}
