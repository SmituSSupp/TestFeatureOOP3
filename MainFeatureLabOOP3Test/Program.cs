using System;
using System.IO;
using System.Globalization;
using System.Text.RegularExpressions;
namespace MainFeatureLabOOP3Test
{
    class Program
    {
        static void Main(string[] args)
        {
           
            string newMessage = "typedef kekw_type int;\r\nint foo_func()\r\n{\r\n    std::cout << \"foo\" << std::endl;\n    return 1;\n}\n\nvoid bar_func(char a)\n{\n    std::cout << \"foo\" << std::endl;\n    return 1;\n}\n\nkekw_type foofoo_func(int a, string test)\n{\n    std::cout << \"foo\" << std::endl;\n    return 1;\n}";
            

            string textFile = @"C:\university\Object_oriented_programming_course\2_sem\lab_3\Source.cpp";
            string text = null;
            if (File.Exists(textFile))
            {
                var sr = new StreamReader(textFile);
                // Read entire text file content in one string    
                text = sr.ReadToEnd();
                //Console.WriteLine(text);
            }


            
            Console.WriteLine("Preparations");
            int foo = 0;
            bool flag_r = false;
            bool quotes = false;
            bool in_comment = false;

            while (text[foo] != '\r' && text[foo+1]!= '\n')
            {
                //Console.WriteLine(text[foo]);
                Console.WriteLine(foo);
                foo++;
            }
            
            Console.WriteLine("newPart");
            Regex rx = new Regex(@"(\S+)\s*(\S+)\s*\((?!\!)(?!\?).*\)\r*\n* *\{");
            Regex multiline_start_rx1 = new Regex(@"\/\*[^,*/]*");
            Regex multiline_end_rx1 = new Regex(@"\*\/");
            Regex one_line_rx1 = new Regex(@"\/\/.*(\\)*");
            //Regex full_str_rx1 = new Regex(".*[^\\]\"");

            

            var result = text.Split("\r\n", StringSplitOptions.None);

            bool next_line_one_line = false;
            string multiline_start_rx =  @"\/\*";
            string multiline_end_rx =  @"\*\/";
            string one_line_rx = @"\/\/.*(\\)*";
            string full_str_rx2 =  "\".*[^\\]\"";
            string full_str_rx = "\".*\"";
            int quote_pos = -1;
            //cycle for one_line comments
            for (int string_pos = 0; string_pos < result.Length; string_pos++)
            {
                if (result[string_pos].Length == 0)
                {
                    continue;
                }
                if (next_line_one_line)
                {
                    next_line_one_line = false;
                    if (result[string_pos][result[string_pos].Length - 1] == '\\')
                    {
                        next_line_one_line = true;
                        Console.WriteLine("Delete next line");
                    }
                    result[string_pos] = result[string_pos].Replace(result[string_pos], " ");
                    
                }


                quote_pos = result[string_pos].IndexOf('"');
                Match match_one_line = Regex.Match(result[string_pos], one_line_rx);
                Match match_multiline_start = Regex.Match(result[string_pos], multiline_start_rx);
                Match match_multiline_end = Regex.Match(result[string_pos], multiline_end_rx);
                int one_line_pos = ((match_one_line.Success) ? match_one_line.Index : -1);
                int multiline_start_pos = ((match_multiline_start.Success) ? match_multiline_start.Index : -1);
                int multiline_end_pos = ((match_multiline_end.Success) ? match_multiline_end.Index : -1);
                
                
                Console.WriteLine(result[string_pos]);
                Console.WriteLine(one_line_pos);


                if (one_line_pos != -1 && multiline_start_pos == -1 && multiline_end_pos == -1 )
                {
                    Match str_test = Regex.Match(result[string_pos], full_str_rx);
                    if (str_test.Success && (one_line_pos > str_test.Index && one_line_pos < (str_test.Index + str_test.Value.Length)))
                    {
                        continue;
                    }
                    else
                    {
                        if (result[string_pos][result[string_pos].Length - 1] == '\\')
                        {
                            next_line_one_line = true;
                            Console.WriteLine("Delete next line");
                        }
                        string thrash_remove = result[string_pos].Substring(one_line_pos);
                        result[string_pos] = result[string_pos].Replace(thrash_remove, "");
                    }
                }
                else if (one_line_pos != -1 && multiline_start_pos != -1 && multiline_end_pos == -1 )
                {
                    if (one_line_pos < multiline_start_pos)
                    {
                        if (result[string_pos][result[string_pos].Length - 1] == '\\')
                        {
                            next_line_one_line = true;
                            Console.WriteLine("Delete next line");
                        }
                        string thrash_remove = result[string_pos].Substring(one_line_pos);
                        result[string_pos] = result[string_pos].Replace(thrash_remove, "");
                    }
                }

            }

            //cycle for multiline search for all openmultiline comments,then close them all and clean strings
            //done only for single open\close ml comments
            bool next_line_mult_line = false;
            bool open_ml_comment = false;
            for (int string_pos = 0; string_pos < result.Length; string_pos++)
            {
                if (result[string_pos].Length == 0)
                {
                    continue;
                }

                quote_pos = result[string_pos].IndexOf('"');

                Match match_multiline_start = Regex.Match(result[string_pos], multiline_start_rx);
                Match match_multiline_end = Regex.Match(result[string_pos], multiline_end_rx);

                 

                int multiline_start_pos = ((match_multiline_start.Success) ? match_multiline_start.Index : -1);
                int multiline_end_pos = ((match_multiline_end.Success) ? match_multiline_end.Index : -1);

                // using this in all code below now instead of simple multiline_end_pos to delete */ too 
                int multiline_end_pos_finish = ((match_multiline_end.Success) ? multiline_end_pos + match_multiline_end.Value.Length:-1);

                Console.WriteLine(result[string_pos]);
                Console.WriteLine(string.Format(CultureInfo.CurrentCulture, "multiline_start_pos = {0} multiline_end_pos = {1} multiline_end_pos_finish = {3} FLAG {2}", multiline_start_pos, multiline_end_pos, open_ml_comment, multiline_end_pos_finish));

                //then  only open /* comment and comment is not open
                if (multiline_start_pos != -1 && multiline_end_pos == -1 && open_ml_comment == false)
                {
                    open_ml_comment = true;
                    string thrash_ml_remove = result[string_pos].Substring(multiline_start_pos);
                    result[string_pos] = result[string_pos].Replace(thrash_ml_remove, "");
                    Console.WriteLine("Delete next line");
                }
                //then line between start and end /* */ comments - must be cleared
                else if (multiline_start_pos == -1 && multiline_end_pos == -1 && open_ml_comment == true)
                {

                    result[string_pos] = result[string_pos].Replace(result[string_pos], "");
                    Console.WriteLine("Delete next line");
                }
                //then line have only closing comment and comment is open - close comment and clear it
                else if (multiline_end_pos != -1 && open_ml_comment == true && multiline_start_pos == -1)
                {
                    string thrash_ml_remove = result[string_pos].Substring(0, multiline_end_pos_finish);
                    result[string_pos] = result[string_pos].Replace(thrash_ml_remove, "");
                    open_ml_comment = false;
                    Console.WriteLine("Delete next line");
                }
                // covering pitfall then we have next situation /* some code
                //                                              /* some code again
                //                                                 closing it here   */
                else if (multiline_start_pos != -1 && open_ml_comment == true && multiline_end_pos == -1)
                {
                    result[string_pos] = result[string_pos].Replace(result[string_pos], "");
                    Console.WriteLine("Delete next line");
                }

                // now to situations must be solved:
                // 1) /*sdsdsdsds*/ then multi line comment is in 1 line
                // 2) foo;*/ int foo1;/* bar; then one comment is closed and next opens again 
                else if (multiline_start_pos != -1 && open_ml_comment == false && multiline_end_pos != -1)
                {
                    if (multiline_start_pos < multiline_end_pos) // situation 1)
                    {
                        string thrash_ml_remove = result[string_pos].Substring(multiline_start_pos, multiline_end_pos_finish- multiline_start_pos);
                        result[string_pos] = result[string_pos].Replace(thrash_ml_remove, "");
                        open_ml_comment = false;
                        Console.WriteLine("Delete next line");
                    }
                    else
                    {
                        Console.WriteLine("THIS SITUATION MUST BE HANDLED " + result[string_pos]);
                    }
                }

                else if (multiline_start_pos != -1 && open_ml_comment == true && multiline_end_pos != -1)
                {
                    //try to manage situation with simple if
                    if (multiline_start_pos < multiline_end_pos) // situation 1)
                    {
                        string thrash_ml_remove = result[string_pos].Substring(0, multiline_end_pos_finish);
                        result[string_pos] = result[string_pos].Replace(thrash_ml_remove, "");
                        open_ml_comment = false;
                        Console.WriteLine("Delete next line");
                    }
                    else if (multiline_start_pos > multiline_end_pos) //situation 2)
                    {
                        string thrash_ml_remove = result[string_pos].Substring(0, multiline_end_pos_finish);
                        result[string_pos] = result[string_pos].Replace(thrash_ml_remove, "");
                        int removed_length = thrash_ml_remove.Length;
                        open_ml_comment = false;
                        thrash_ml_remove = result[string_pos].Substring(multiline_start_pos-removed_length);
                        result[string_pos] = result[string_pos].Replace(thrash_ml_remove, "");
                        open_ml_comment = true;
                        Console.WriteLine("Delete next line");

                    }
                    else
                    {
                        Console.WriteLine("THIS SITUATION MUST BE HANDLED "+result[string_pos]);
                    }
                }
            }
            System.IO.File.WriteAllLines(@"C:\university\Object_oriented_programming_course\2_sem\lab_3\after_cleaning.cpp", result);

            /*
            foreach (Match match in multiline_rx.Matches(text))
            {

                int index = match.Index;
                string matched_str = match.Value;
                int helper = index;
                Console.WriteLine(matched_str);
                Console.WriteLine(index);
            }*/

            //Match match = Regex.Match(newMessage, pattern);
            /*
            foreach (Match match in rx.Matches(text))
            {

                int index = match.Index;
                string matched_str = match.Value;
                int helper = index;
                Console.WriteLine(matched_str);
                Console.WriteLine(index);
                string test_message = string.Format(CultureInfo.CurrentCulture, "MATCHED STRING: {0} POSITION {1}", matched_str, index);
                while (text[helper] != '{')
                {
                    
                    Console.Write(text[helper]);
                    helper++;
                }
                //Console.WriteLine(test_message);
                */


        }
    }
}
