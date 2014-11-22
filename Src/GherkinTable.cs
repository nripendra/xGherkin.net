using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xGherkin
{
    public class GherkinTable : ICollection<System.Data.DataRow>
    {
        public System.Data.DataTable Table { get; private set; }

        public GherkinTable(params string[] headers)
        {
            Table = new System.Data.DataTable();
            for (int i = 0; i < headers.Length; i++)
            {
                Table.Columns.Add(headers[i]);
            }
        }

        public IEnumerable<System.Data.DataRow> Rows
        {
            get
            {
                return Table.Rows.Cast<System.Data.DataRow>().AsEnumerable();
            }
        }

        public IEnumerable<System.Data.DataColumn> Cols
        {
            get
            {
                return Table.Columns.Cast<System.Data.DataColumn>().AsEnumerable();
            }
        }

        public void Add(params object[] items)
        {
            if (items.Length != Table.Columns.Count)
                throw new ArgumentException(string.Format("Number of column values does not match number of headers, got {0}, expected {1}", items.Length, Table.Columns.Count));

            Table.Rows.Add(items);

        }

        public void Add(System.Data.DataRow item)
        {
            Table.Rows.Add(item);
        }

        public void Clear()
        {
            Table.Rows.Clear();
        }

        public bool Contains(System.Data.DataRow item)
        {
            return Table.Rows.Contains(item);
        }

        public void CopyTo(System.Data.DataRow[] array, int arrayIndex)
        {
            Table.Rows.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return Table.Rows.Count; }
        }

        public bool IsReadOnly
        {
            get { return Table.Rows.IsReadOnly; }
        }

        public bool Remove(System.Data.DataRow item)
        {
            Table.Rows.Remove(item);
            return true;
        }

        public IEnumerator<System.Data.DataRow> GetEnumerator()
        {
            return (IEnumerator<System.Data.DataRow>)Table.Rows.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Table.Rows.GetEnumerator();
        }

        public virtual string ToConsoleString()
        {
            int[] colLength = new int[Table.Columns.Count];

            foreach (System.Data.DataRow row in Table.Rows)
            {

                for (int i = 0; i < Table.Columns.Count; i++)
                {
                    int len = row[i].ToString().Length;
                    if (colLength[i] < len)
                    {
                        colLength[i] = len;
                    }
                }

            }

            StringBuilder str = new StringBuilder("|");

            for (int i = 0; i < Table.Columns.Count; i++)
            {
                int len = Table.Columns[i].ColumnName.Length;
                if (colLength[i] < len)
                {
                    colLength[i] = len;
                }

                str.Append(Truncate(Table.Columns[i].ColumnName, colLength[i])).Append("|");
            }

            str.AppendLine();

            foreach (System.Data.DataRow row in Table.Rows)
            {
                str.Append("|");

                for (int i = 0; i < Table.Columns.Count; i++)
                {
                    str.Append(Truncate(row[i].ToString(), colLength[i])).Append("|");
                }

                str.AppendLine();
            }

            return str.ToString();
        }

        private string Truncate(string str, int length)
        {
            if (str.Length > length)
            {
                return str.Substring(0, length);
            }

            int len = length - str.Length;
            for (int i = 0; i < len; i++)
            {
                str += '\u3000';
            }
            return str;
        }
    }
}
