using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.CSharp;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Reflection;

using System.IO;

using System.Security;
using System.Security.Permissions;
using System.Security.Policy;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GlyphEngine.Scripting
{
    public abstract class BaseScript
    {
        #region Members

        private Assembly _assembly = null;
        private Dictionary<string, MethodInfo> _methods = new Dictionary<string, MethodInfo>();

        private static List<string> _usings = null;
        private static PermissionSet _scriptPermissions = null;
        private static CodeDomProvider _compiler = null;
        private static CompilerParameters _compilerParameters = null;

        #endregion Members

        static BaseScript()
        {
            _scriptPermissions = new PermissionSet(PermissionState.None);
            _scriptPermissions.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));
            _scriptPermissions.AddPermission(new ReflectionPermission(ReflectionPermissionFlag.NoFlags));

            _compiler = CodeDomProvider.CreateProvider("CSharp");

            _compilerParameters = new CompilerParameters();
            _compilerParameters.GenerateInMemory = true;
            _compilerParameters.GenerateExecutable = false;
            _compilerParameters.IncludeDebugInformation = false;
            _compilerParameters.TreatWarningsAsErrors = false;

            _compilerParameters.ReferencedAssemblies.Add(typeof(int).Assembly.Location);            
            _compilerParameters.ReferencedAssemblies.Add(typeof(Microsoft.Xna.Framework.Game).Assembly.Location);
            _compilerParameters.ReferencedAssemblies.Add(typeof(BaseScript).Assembly.Location);

            _usings = new List<string>();
            _usings.Add("System");
            _usings.Add("Microsoft.Xna.Framework");
            _usings.Add("GlyphEngine");
        }

        public void Load(string filename)
        {
            Code = string.Empty;

            var cType = this.GetType();
            var nameSpace = cType.Namespace;
            var className = cType.Name;
            var content = File.ReadAllText(filename);

            var sb = new StringBuilder();

            foreach (string u in _usings)
                sb.Append(string.Format("using {0};", u));

            sb.Append(string.Format("namespace {0} {", nameSpace));
            sb.Append(string.Format("public class {0} { {1} } }", className, content));

            this.Code = sb.ToString();

            Compile();

            this.Filename = filename;
        }

        private void Compile()
        {
            try
            {
                Type cType = this.GetType();
                _compilerParameters.ReferencedAssemblies.Add(cType.Assembly.Location);

                CompilerResults results = _compiler.CompileAssemblyFromSource(_compilerParameters, Code);

                var sb = new StringBuilder();
                if (results.Errors.HasErrors)
                {
                    foreach (CompilerError err in results.Errors)
                    {
                        sb.Append(err.ToString());
                        sb.Append("\r\n");
                    }

                    throw new Exception(sb.ToString());
                }

                _assembly = results.CompiledAssembly;

                var objectMethods = new HashSet<string>();
                objectMethods.Add("ToString");
                objectMethods.Add("GetType");
                objectMethods.Add("Equals");
                objectMethods.Add("GetHashCode");

                _methods.Clear();
                Type[] types = _assembly.GetExportedTypes();
                foreach (Type t in types)
                {
                    foreach (MethodInfo m in t.GetMethods())
                    {
                        if (!objectMethods.Contains(m.Name))
                            _methods.Add(m.Name, m);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected void Execute(string methodName, params object[] args)
        {
            if (!_methods.ContainsKey(methodName))
                return;

            _scriptPermissions.PermitOnly();

            _methods[methodName].Invoke(null, args);

            CodeAccessPermission.RevertPermitOnly();
        }

        #region Properties

        public string Filename { get; private set; }

        public string Code { get; protected set; }

        #endregion Properties
    }
}
