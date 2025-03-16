CREATE INDEX IF NOT EXISTS idx_sales_opportunity_tenant_id
ON sales_opportunity (tenant_id, customer_id, lead_id, product_id);