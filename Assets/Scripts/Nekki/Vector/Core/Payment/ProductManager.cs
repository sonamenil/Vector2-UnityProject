using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace Nekki.Vector.Core.Payment
{
	public class ProductManager
	{
		public Action OnProductsUpdate;

		public Action OnProductsUpdateFailed;

		private bool _ProductWasRecived;

		private List<string> _AllIds;

		private List<Product> _AllProducts;

		private Dictionary<string, Product> _ProductById;

		private Dictionary<string, List<Product>> _ProductByGroup;

		private bool _IsGetProductDataTransactionCompleted;

		private static string ProductFilePath
		{
			get
			{
				return VectorPaths.InApps + ((!ApplicationController.IsPaidBundleID) ? "/android_products.xml" : "/android_paid_products.xml");
			}
		}

		public static ProductManager Current { get; private set; }

		public bool ProductWasRecived
		{
			get
			{
				return _ProductWasRecived;
			}
		}

		public bool IsGetProductDataTransactionCompleted
		{
			get
			{
				return _IsGetProductDataTransactionCompleted;
			}
		}

		private ProductManager()
		{
			_AllIds = new List<string>();
			_AllProducts = new List<Product>();
			_ProductById = new Dictionary<string, Product>();
			_ProductByGroup = new Dictionary<string, List<Product>>();
			if (PaymentController.Current != null)
			{
				PaymentAbstract current = PaymentController.Current;
				current.OnProductsListRequestSuccess = (Action<List<Product>>)Delegate.Combine(current.OnProductsListRequestSuccess, new Action<List<Product>>(OnProductListRequestSuccess));
				PaymentAbstract current2 = PaymentController.Current;
				current2.OnProductsListRequestFalied = (Action<string>)Delegate.Combine(current2.OnProductsListRequestFalied, new Action<string>(OnProductListRequestFalied));
			}
		}

		public static void Init()
		{
			if (Current == null)
			{
				Current = new ProductManager();
				Current.LoadProducts(ProductFilePath);
				Current.LoadProducts(VectorPaths.InApps + "/external_products.xml");
				Current.GetProductsData();
			}
		}

		public static void Free()
		{
			if (Current != null)
			{
				Current = null;
			}
		}

		public List<string> GetAllIds()
		{
			return _AllIds;
		}

		public List<Product> GetAllProducts()
		{
			return _AllProducts;
		}

		public Product GetProductById(string p_id)
		{
			Product value = null;
			_ProductById.TryGetValue(p_id, out value);
			return value;
		}

		public List<Product> GetProductsByGroup(string p_groupName)
		{
			List<Product> value = null;
			_ProductByGroup.TryGetValue(p_groupName, out value);
			return value;
		}

		public List<string> GetNonConsumableIds()
		{
			List<string> list = new List<string>();
			foreach (Product allProduct in _AllProducts)
			{
				if (!allProduct.IsConsumable)
				{
					list.Add(allProduct.Id);
				}
			}
			return list;
		}

		public List<string> GetConsumableIds()
		{
			List<string> list = new List<string>();
			foreach (Product allProduct in _AllProducts)
			{
				if (allProduct.IsConsumable)
				{
					list.Add(allProduct.Id);
				}
			}
			return list;
		}

		public List<Product> GetConsumableProducts()
		{
			List<Product> list = new List<Product>();
			foreach (Product allProduct in _AllProducts)
			{
				if (allProduct.IsConsumable)
				{
					list.Add(allProduct);
				}
			}
			return list;
		}

		public List<Product> GetNonConsumableProducts()
		{
			List<Product> list = new List<Product>();
			foreach (Product allProduct in _AllProducts)
			{
				if (!allProduct.IsConsumable)
				{
					list.Add(allProduct);
				}
			}
			return list;
		}

		public bool IsProductConsumable(string p_id, bool p_def = false)
		{
			Product productById = GetProductById(p_id);
			if (productById == null)
			{
				return p_def;
			}
			return productById.IsConsumable;
		}

		public void GetProductsData()
		{
			GetProductsData(_AllIds.ToArray());
		}

		public void GetProductsData(params string[] p_products)
		{
			if (PaymentController.Current != null)
			{
				_IsGetProductDataTransactionCompleted = false;
				PaymentController.Current.GetProductsData(p_products);
			}
		}

		public void UpdateProductsAvaliable()
		{
			foreach (Product allProduct in _AllProducts)
			{
				allProduct.UpdateAvaliable();
			}
		}

		~ProductManager()
		{
			if (PaymentController.Current != null)
			{
				PaymentAbstract current = PaymentController.Current;
				current.OnProductsListRequestSuccess = (Action<List<Product>>)Delegate.Remove(current.OnProductsListRequestSuccess, new Action<List<Product>>(OnProductListRequestSuccess));
				PaymentAbstract current2 = PaymentController.Current;
				current2.OnProductsListRequestFalied = (Action<string>)Delegate.Remove(current2.OnProductsListRequestFalied, new Action<string>(OnProductListRequestFalied));
			}
		}

		private void LoadProducts(string p_file)
		{
			XmlDocument xmlDocument = XmlUtils.OpenXMLDocument(p_file, string.Empty);
			XmlNode xmlNode = xmlDocument["Root"];
			foreach (XmlNode childNode in xmlNode.ChildNodes)
			{
				Product p_product = new Product(childNode);
				AddProduct(p_product);
			}
		}

		private void AddProduct(Product p_product)
		{
			if (!p_product.IsFree)
			{
				_AllIds.Add(p_product.Id);
			}
			_AllProducts.Add(p_product);
			_ProductById.Add(p_product.Id, p_product);
			List<Product> value = null;
			if (!_ProductByGroup.TryGetValue(p_product.Group, out value))
			{
				value = (_ProductByGroup[p_product.Group] = new List<Product>());
			}
			value.Add(p_product);
		}

		private void ReplaceProductsData(params Product[] p_products)
		{
			foreach (Product product in p_products)
			{
				Product productById = GetProductById(product.Id);
				if (productById != null)
				{
					productById.ReplaceData(product);
				}
			}
		}

		private void OnProductListRequestSuccess(List<Product> p_products)
		{
			_IsGetProductDataTransactionCompleted = true;
			if (p_products.Count != 0 || DeviceInformation.IsEmulator)
			{
				_ProductWasRecived = true;
				ReplaceProductsData(p_products.ToArray());
				if (OnProductsUpdate != null)
				{
					OnProductsUpdate();
				}
			}
		}

		private void OnProductListRequestFalied(string p_error)
		{
			_IsGetProductDataTransactionCompleted = true;
			if (OnProductsUpdateFailed != null)
			{
				OnProductsUpdateFailed();
			}
			Debug.Log("[BaseProductManager]: can't validate products info - " + p_error);
		}
	}
}
