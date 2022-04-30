/****** Script for SelectTopNRows command from SSMS  ******/
SELECT [RowNum]
	,PurchaseOrders.Id
	,PurchaseOrders.Number
	,PurchaseOrders.Date
      ,[ProductId]
	  ,Products.Name
      ,[Qty]
      ,[Price]
      ,[Total]
  FROM [ConsoleApp1.StoreDbContext].[dbo].[PurchaseOrderItems] as [PurchaseOrderItems]
  inner join PurchaseOrders as PurchaseOrders on PurchaseOrderItems.ParentId = PurchaseOrders.Id
  left join Products on PurchaseOrderItems.ProductId = Products.Id