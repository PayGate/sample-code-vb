'
' Copyright (c) 2018 PayGate (Pty) Ltd
'
' Author: App Inlet (Pty) Ltd
'
' Released under the GNU General Public License
'
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Text

Public Module GlobalUtility
    Function generateReference() As String
        Return "pgtest_" & DateTime.Now.ToString("yyyyMMddHHmmss")
    End Function

    Function getUriParts() As String()
        Dim url As String() = (HttpContext.Current.Request.Url.AbsoluteUri).Split("/"c)
        Return url
    End Function

    Function getScheme() As String
        Dim uri As Uri = New Uri(HttpContext.Current.Request.Url.AbsoluteUri)
        Return uri.Scheme
    End Function

    Function getHost() As String
        Dim uri As Uri = New Uri(HttpContext.Current.Request.Url.AbsoluteUri)
        Return uri.Authority
    End Function

    Public Function generateCountrySelectOptions() As String
        Dim options As StringBuilder = New StringBuilder()
        Dim country As String = "ZAF"
        Dim mostUsedCountryArray As Dictionary(Of String, String) = New Dictionary(Of String, String)() From {
            {"DEU", "Germany"},
            {"ZAF", "South Africa"},
            {"USA", "United States"}
        }
        Dim countryArray As Dictionary(Of String, String) = New Dictionary(Of String, String)() From {
            {"AFG", "Afghanistan"},
            {"ALB", "Albania"},
            {"DZA", "Algeria"},
            {"ASM", "American Samoa"},
            {"AND", "Andorra"},
            {"AGO", "Angola"},
            {"AIA", "Anguilla"},
            {"ATA", "Antarctica"},
            {"ATG", "Antigua and Barbuda"},
            {"ARG", "Argentina"},
            {"ARM", "Armenia"},
            {"ABW", "Aruba"},
            {"AUS", "Australia"},
            {"AUT", "Austria"},
            {"AZE", "Azerbaijan"},
            {"BHS", "Bahamas"},
            {"BHR", "Bahrain"},
            {"BGD", "Bangladesh"},
            {"BRB", "Barbados"},
            {"BLR", "Belarus"},
            {"BEL", "Belgium"},
            {"BLZ", "Belize"},
            {"BEN", "Benin"},
            {"BMU", "Bermuda"},
            {"BTN", "Bhutan"},
            {"BOL", "Bolivia"},
            {"BIH", "Bosnia and Herzegovina"},
            {"BWA", "Botswana"},
            {"BVT", "Bouvet Island"},
            {"BRA", "Brazil"},
            {"IOT", "British Indian Ocean Territory"},
            {"VGB", "British Virgin Islands"},
            {"BRN", "Brunei Darussalam"},
            {"BGR", "Bulgaria"},
            {"BFA", "Burkina Faso"},
            {"BDI", "Burundi"},
            {"KHM", "Cambodia"},
            {"CMR", "Cameroon"},
            {"CAN", "Canada"},
            {"CPV", "Cape Verde"},
            {"CYM", "Cayman Islands"},
            {"CAF", "Central African Republic"},
            {"TCD", "Chad"},
            {"CHL", "Chile"},
            {"CHN", "China"},
            {"CXR", "Christmas Island"},
            {"CCK", "Cocos (Keeling) Islands"},
            {"COL", "Colombia"},
            {"COM", "Comoros"},
            {"COG", "Congo"},
            {"COD", "Congo The Democratic Republic of The"},
            {"COK", "Cook Islands"},
            {"CRI", "Costa Rica"},
            {"CIV", "Cote D'ivoire"},
            {"CHRV", "Croatia"},
            {"CUB", "Cuba"},
            {"CYP", "Cyprus"},
            {"CZE", "Czech Republic"},
            {"DNK", "Denmark"},
            {"DJI", "Djibouti"},
            {"DMA", "Dominica"},
            {"DOM", "Dominican Republic"},
            {"ECU", "Ecuador"},
            {"EGY", "Egypt"},
            {"SLV", "El Salvador"},
            {"GNQ", "Equatorial Guinea"},
            {"ERI", "Eritrea"},
            {"EST", "Estonia"},
            {"ETH", "Ethiopia"},
            {"FLK", "Falkland Islands (Malvinas)"},
            {"FRO", "Faroe Islands"},
            {"FJI", "Fiji"},
            {"FIN", "Finland"},
            {"FRA", "France"},
            {"GUF", "French Guiana"},
            {"FXX", "French Metropolitan"},
            {"PYF", "French Polynesia"},
            {"ATF", "French Southern Territories"},
            {"GAB", "Gabon"},
            {"GMB", "Gambia"},
            {"GEO", "Georgia"},
            {"DEU", "Germany"},
            {"GHA", "Ghana"},
            {"GIB", "Gibraltar"},
            {"GRC", "Greece"},
            {"GRL", "Greenland"},
            {"GRD", "Grenada"},
            {"GLP", "Guadeloupe"},
            {"GUM", "Guam"},
            {"GTM", "Guatemala"},
            {"GIN", "Guinea"},
            {"GNB", "Guinea-bissau"},
            {"GUY", "Guyana"},
            {"HTI", "Haiti"},
            {"HMD", "Heard Island and Mcdonald Islands"},
            {"VAT", "Holy See (Vatican City State)"},
            {"HND", "Honduras"},
            {"HKG", "Hong Kong"},
            {"HUN", "Hungary"},
            {"ISL", "Iceland"},
            {"IND", "India"},
            {"IDN", "Indonesia"},
            {"IRN", "Iran Islamic Republic of"},
            {"IRQ", "Iraq"},
            {"IRL", "Ireland"},
            {"ISR", "Israel"},
            {"ITA", "Italy"},
            {"JAM", "Jamaica"},
            {"JPN", "Japan"},
            {"JOR", "Jordan"},
            {"KAZ", "Kazakhstan"},
            {"KEN", "Kenya"},
            {"KIR", "Kiribati"},
            {"PRK", "Korea Democratic People's Republic of"},
            {"KOR", "Korea Republic of"},
            {"KWT", "Kuwait"},
            {"KGZ", "Kyrgyzstan"},
            {"LAO", "Lao People's Democratic Republic"},
            {"LVA", "Latvia"},
            {"LBN", "Lebanon"},
            {"LSO", "Lesotho"},
            {"LBR", "Liberia"},
            {"LBY", "Libyan Arab Jamahiriya"},
            {"LIE", "Liechtenstein"},
            {"LTU", "Lithuania"},
            {"LUX", "Luxembourg"},
            {"MAC", "Macau China"},
            {"MKD", "Macedonia The Former Yugoslav Republic of"},
            {"MDG", "Madagascar"},
            {"MWI", "Malawi"},
            {"MYS", "Malaysia"},
            {"MDV", "Maldives"},
            {"MLI", "Mali"},
            {"MLT", "Malta"},
            {"MHL", "Marshall Islands"},
            {"MTQ", "Martinique"},
            {"MRT", "Mauritania"},
            {"MUS", "Mauritius"},
            {"MYT", "Mayotte"},
            {"MEX", "Mexico"},
            {"FSM", "Micronesia Federated States of"},
            {"MDA", "Moldova Republic of"},
            {"MCO", "Monaco"},
            {"MNG", "Mongolia"},
            {"MSR", "Montserrat"},
            {"MAR", "Morocco"},
            {"MOZ", "Mozambique"},
            {"MMR", "Myanmar"},
            {"NAM", "Namibia"},
            {"NRU", "Nauru"},
            {"NPL", "Nepal"},
            {"NLD", "Netherlands"},
            {"ANT", "Netherlands Antilles"},
            {"NCL", "New Caledonia"},
            {"NZL", "New Zealand"},
            {"NIC", "Nicaragua"},
            {"NER", "Niger"},
            {"NGA", "Nigeria"},
            {"NIU", "Niue"},
            {"NFK", "Norfolk Island"},
            {"MNP", "Northern Mariana Islands"},
            {"NOR", "Norway"},
            {"OMN", "Oman"},
            {"PAK", "Pakistan"},
            {"PLW", "Palau"},
            {"PAN", "Panama"},
            {"PNG", "Papua New Guinea"},
            {"PRY", "Paraguay"},
            {"PER", "Peru"},
            {"PHL", "Philippines"},
            {"PCN", "Pitcairn"},
            {"POL", "Poland"},
            {"PRT", "Portugal"},
            {"PRI", "Puerto Rico"},
            {"QAT", "Qatar"},
            {"REU", "Reunion"},
            {"ROM", "Romania"},
            {"RUS", "Russian Federation"},
            {"RWA", "Rwanda"},
            {"SHN", "Saint Helena"},
            {"KNA", "Saint Kitts and Nevis"},
            {"LCA", "Saint Lucia"},
            {"SPM", "Saint Pierre and Miquelon"},
            {"VCT", "Saint Vincent and The Grenadines"},
            {"WSM", "Samoa"},
            {"SMR", "San Marino"},
            {"STP", "Sao Tome and Principe"},
            {"SAU", "Saudi Arabia"},
            {"SEN", "Senegal"},
            {"SYC", "Seychelles"},
            {"SLE", "Sierra Leone"},
            {"SGP", "Singapore"},
            {"SVK", "Slovakia"},
            {"SVN", "Slovenia"},
            {"SLB", "Solomon Islands"},
            {"SOM", "Somalia"},
            {"ZAF", "South Africa"},
            {"SGS", "South Georgia and The South Sandwich Islands"},
            {"ESP", "Spain"},
            {"LKA", "Sri Lanka"},
            {"SDN", "Sudan"},
            {"SUR", "Suriname"},
            {"SJM", "Svalbard and Jan Mayen"},
            {"SWZ", "Swaziland"},
            {"SWE", "Sweden"},
            {"CHE", "Switzerland"},
            {"SYR", "Syrian Arab Republic"},
            {"TWN", "Taiwan Province of China"},
            {"TJK", "Tajikistan"},
            {"TZA", "Tanzania United Republic of"},
            {"THA", "Thailand"},
            {"TGO", "Togo"},
            {"TKL", "Tokelau"},
            {"TON", "Tonga"},
            {"TTO", "Trinidad and Tobago"},
            {"TUN", "Tunisia"},
            {"TUR", "Turkey"},
            {"TKM", "Turkmenistan"},
            {"TCA", "Turks and Caicos Islands"},
            {"TUV", "Tuvalu"},
            {"VIR", "U.S. Virgin Islands"},
            {"UGA", "Uganda"},
            {"UKR", "Ukraine"},
            {"ARE", "United Arab Emirates"},
            {"GBR", "United Kingdom"},
            {"USA", "United States"},
            {"UMI", "United States Minor Outlying Islands"},
            {"URY", "Uruguay"},
            {"UZB", "Uzbekistan"},
            {"VUT", "Vanuatu"},
            {"VEN", "Venezuela"},
            {"VNM", "Vietnam"},
            {"WLF", "Wallis and Futuna"},
            {"ESH", "Western Sahara"},
            {"YEM", "Yemen"},
            {"YUG", "Yugoslavia"},
            {"ZMB", "Zambia"},
            {"ZWE", "Zimbabwe"}
        }
        Dim defaultSet As Boolean = False
        options.Append("<optgroup label=""""><option value="""">Select Country</option>")
        options.Append("<optgroup label=""Most Used"">")

        For Each entry As KeyValuePair(Of String, String) In mostUsedCountryArray
            options.Append("<option value=""" & entry.Key & """")

            If country = entry.Key AndAlso Not defaultSet Then
                options.Append(" selected=""selected""")
                defaultSet = True
            End If

            options.Append(">" & entry.Value & "</option>")
        Next

        options.Append("</optgroup>")
        options.Append("<optgroup label=""All Countries"">")

        For Each entry As KeyValuePair(Of String, String) In countryArray
            options.Append("<option value=""")
            options.Append(entry.Key)
            options.Append("""")

            If country = entry.Key AndAlso Not defaultSet Then
                options.Append(" selected=""selected""")
                defaultSet = True
            End If

            options.Append(">")
            options.Append(entry.Value)
            options.Append("</option>")
        Next

        options.Append("</optgroup>")
        Return options.ToString()
    End Function
    Public Function getStringArray(ByVal input As String) As String()
        Dim output = New String(0) {}
        output(0) = input
        Return output
    End Function
End Module
