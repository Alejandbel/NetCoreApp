using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebLab.Domain.Models
{
	public class ResponseData<T>
	{

		public ResponseData(T data)
		{
			Data = data;
		}

		public T Data { get; set; }
		public bool IsSuccess { get; set; } = true;
		public string? ErrorMessage { get; set; }
	}
}
