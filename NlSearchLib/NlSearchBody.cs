namespace NewsLibrarySearch
{
    /// <summary>
    /// Builder for NewsLibrary request body
    /// </summary>
    public class NlSearchBody
    {
        public string Body { get; private set; }

        public NlSearchBody(string date, string searchTerm, string fieldTarget)
        {
            if (date != "")
                date = "&p_text_YMD_date-0=" + date;
            string bodyHeader = @"s_siteloc=NL2&p_queryname=4000&p_action=search&p_product=NewsLibrary&p_theme=newslibrary2&s_search_type=customized&d_sources=location&d_place=United+States&p_nbid=&p_field_psudo-sort-0=psudo-sort&f_multi=&p_multi=";
            string bodyFooter = "&p_widesearch=smart&p_sort=YMD_date%3AD&p_maxdocs=200&p_perpage=10&p_text_base-0=" + searchTerm + "&p_field_base-0=" + fieldTarget + "&p_bool_base-1=AND&p_text_base-1=&p_field_base-1=Section&p_bool_base-2=AND&p_text_base-2=" + "&p_field_base-2=" + date + "&p_field_YMD_date-0=YMD_date&p_params_YMD_date-0=date%3AB%2CE&p_field_YMD_date-3=YMD_date&p_params_YMD_date-3=date%3AB%2CE&Search.x=18&Search.y=18";
            Body = bodyHeader + bodyFooter;
        }
    }
}
