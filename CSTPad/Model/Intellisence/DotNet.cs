using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CSTPad.Model.Intellisence
{
    public class DotNet
    {
        public static IEnumerable<string> GetTypes(IEnumerable<string> usings, string word)
        {
            if (!string.IsNullOrWhiteSpace(word))
            {
                var asms = AppDomain.CurrentDomain.GetAssemblies();
                var corelib = Assembly.Load("mscorlib.dll");
                var sys = asms.FirstOrDefault(x => x.GetName().Name == "System");
                var linq = asms.FirstOrDefault(x => x.GetName().Name == "System.Linq");

                List<Assembly> searchAsmList = new List<Assembly>();
                searchAsmList.Add(corelib);
                searchAsmList.Add(sys);
                searchAsmList.Add(linq);

                List<string> nameList = new List<string>();
                foreach (var asm in searchAsmList)
                {
                    foreach (var type in asm.ExportedTypes)
                    {
                        foreach (var @using in usings.Where(x => !string.IsNullOrWhiteSpace(x)))
                        {
                            bool isContainsNamespace = type.FullName.Contains(@using);
                            string nameWithoutNamespace = type.FullName.Replace(@using, string.Empty).TrimStart('.');

                            if (isContainsNamespace && !nameWithoutNamespace.Contains("."))
                            {
                                nameList.Add(nameWithoutNamespace);
                            }
                        }
                    }
                }

                foreach (var name in nameList)
                {
                    string regex = "^" + Regex.Escape(word);
                    StringBuilder sb = new StringBuilder();
                    foreach (var ch in regex)
                    {
                        if (Regex.IsMatch(ch.ToString(), "[a-zA-Z_]"))
                        {
                            sb.Append(ch).Append(".*");
                        }
                        else
                        {
                            sb.Append(ch);
                        }
                    }

                    regex = sb.ToString();

                    if (Regex.IsMatch(name, regex, RegexOptions.IgnoreCase) && !name.Contains("`"))
                    {
                        yield return name;
                    }
                }
            }
        }
    }
}
