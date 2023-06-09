namespace IDVerification.Models
{
    public class ResultSource
    {
        public string name { get; set; }
        public string label { get; set; }
        public string address { get; set; }
        public string birth_date { get; set; }
        public string national_id_type { get; set; }
    }

    public class FaceMatchOcrData
    {
        public string city { get; set; }
        public string state { get; set; }
        public string unit_no { get; set; }
        public string full_name { get; set; }
        public string last_name { get; set; }
        public DateTime birth_date { get; set; }
        public string first_name { get; set; }
        public DateTime issue_date { get; set; }
        public DateTime expiry_date { get; set; }
        public string middle_name { get; set; }
        public string national_id { get; set; }
        public string postal_code { get; set; }
        public string country_code { get; set; }
        public string country_name { get; set; }
        public string address_line_1 { get; set; }
        public string address_line_2 { get; set; }
        public string national_id_type { get; set; }
        public string national_id_tertiary { get; set; }
        public string national_id_secondary { get; set; }
        public string national_id_tertiary_type { get; set; }
        public string national_id_secondary_type { get; set; }
    }

    public class IDVResult
    {
        public string signature_key { get; set; }
        public string url { get; set; }
        public string type { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string first_name { get; set; }
        public string middle_name { get; set; }
        public string last_name { get; set; }
        public DateTime birth_date { get; set; }
        public string country { get; set; }
        public string unit_no { get; set; }
        public string address_line_1 { get; set; }
        public string address_line_2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string postal_code { get; set; }
        public double? latitude { get; set; }
        public double? longitude { get; set; }
        public string national_id { get; set; }
        public string national_id_secondary { get; set; }
        public string national_id_tertiary { get; set; }
        public string national_id_type { get; set; }
        public string national_id_secondary_type { get; set; }
        public string national_id_tertiary_type { get; set; }
        public DateTime created_at { get; set; }
        public bool is_cancelled { get; set; }
        public bool is_completed { get; set; }
        public bool is_id_verification_completed { get; set; }
        public bool is_quick_id_completed { get; set; }
        public bool quick_id_is_completed { get; set; }
        public List<string> unique_ip_addresses { get; set; }
        public DateTime quick_id_completed_at { get; set; }
        public DateTime facematch_completed_at { get; set; }
        public DateTime completed_at { get; set; }
        public string final_report_pdf_url_TEMP { get; set; }
        public string report_pdf_url_TEMP { get; set; }
        public string result { get; set; }
        public List<ResultSource> result_sources { get; set; }
        public List<ResultSource> quick_id_result_sources { get; set; }
        public string quick_id_name_result { get; set; }
        public string quick_id_birth_date_result { get; set; }
        public string quick_id_address_result { get; set; }
        public string quick_id_overall_result { get; set; }
        public string pep_overall_result { get; set; }
        public string name_result { get; set; }
        public string birth_date_result { get; set; }
        public string address_result { get; set; }
        public string overall_result { get; set; }
        public string pep_result { get; set; }
        public string pep_hits { get; set; }
        public string org_name { get; set; }
        public bool is_facematch_completed { get; set; }
        public bool facematch_is_completed { get; set; }
        public string facematch_identity_document_result { get; set; }
        public string facematch_data_comparison_result { get; set; }
        public string facematch_document_expiry_result { get; set; }
        public string facematch_anti_tamper_result { get; set; }
        public string facematch_id_doc_liveliness_result { get; set; }
        public string facematch_agency_duplicate_result { get; set; }
        public string facematch_anti_fraud_result { get; set; }
        public string facematch_portraits_comparison_result { get; set; }
        public string facematch_mrz_result { get; set; }
        public string facematch_portrait_age_result { get; set; }
        public string facematch_public_figure_result { get; set; }
        public string facematch_photo_liveliness_result { get; set; }
        public string facematch_face_comparison_result { get; set; }
        public string facematch_overall_result { get; set; }
        public bool facematch_first_name_OCR_result { get; set; }
        public bool facematch_last_name_OCR_result { get; set; }
        public bool facematch_birth_date_OCR_result { get; set; }
        public bool facematch_document_OCR_check { get; set; }
        public string facematch_matched_public_figures { get; set; }
        public string facematch_portrait_age_range { get; set; }
        public string facematch_duplicate_signature_keys { get; set; }
        public string facematch_id_type { get; set; }
        public string facematch_id_country { get; set; }
        public DateTime? facematch_id_expiry { get; set; }
        public FaceMatchOcrData facematch_ocr_data { get; set; }
        public bool? facematch_mrz_validated { get; set; }
        public string facematch_id_front_url_TEMP { get; set; }
        public string facematch_id_back_url_TEMP { get; set; }
        public string facematch_id_front_original_url_TEMP { get; set; }
        public string facematch_id_back_original_url_TEMP { get; set; }
        public string facematch_face_from_id_url_TEMP { get; set; }
        public string facematch_face_from_selfie_url_TEMP { get; set; }
        public string facematch_face_from_liveness_url_TEMP { get; set; }
        public string facematch_selfie_url_TEMP { get; set; }
        public string facematch_liveness_url_TEMP { get; set; }
        public string facematch_liveness_video1_url_TEMP { get; set; }
        public string facematch_liveness_video2_url_TEMP { get; set; }
        public int facematch_liveness_probability { get; set; }
        public int facematch_liveness_quality { get; set; }
        public bool? is_bankmatch_completed { get; set; }
        public bool? bankmatch_is_completed { get; set; }
        public string bankmatch_bank_name { get; set; }
        public string bankmatch_bank_logo_url_TEMP { get; set; }
        public string bankmatch_overall_result { get; set; }
        public string facematch_id_front_o { get; set; }
        public string facematch_id_front_c { get; set; }
        public string facematch_liveness_o { get; set; }
        public string facematch_liveness_c { get; set; }
    }
}

