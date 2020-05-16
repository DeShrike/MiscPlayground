namespace OutilacTest
{
    using System.Collections.Generic;

    public class BodyBuilder
    {
        private Stack<string> elements = new Stack<string>();
        private string html = "";
        private int indent = 0;

        public BodyBuilder()
        {
            this.html = string.Empty;
            indent = 0;
        }

        public string Content { get { return this.html; } }

        public string AddIndents()
        {
            for (int i = 0; i < this.indent; i++)
            {
                this.html += "  ";
            }

            return this.html;
        }

        public string StartElement(string name)
        {
            this.elements.Push(name);
            html = this.AddIndents();
            html = html + "<" + name + ">" + "\r\n";
            indent += 1;

            return html;
        }

        public string EndElement()
        {
            var name = elements.Pop();
            this.indent -= 1;
            this.html = this.AddIndents();
            this.html = this.html + "</" + name + ">" + "\r\n";
            return html;
        }

        public string AddElementAndValue(string name, string value)
        {
            html = this.AddIndents();
            html = html + "<" + name + ">" + value + "</" + name + ">" + "\r\n";
            return html;
        }
    }
}
