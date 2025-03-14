CREATE OR REPLACE FUNCTION get_customer_relationships(customer_id INT) RETURNS JSON AS $$
DECLARE
    result JSONB := '{}'::JSONB; -- Initialize result as an empty JSONB object
    entity_name TEXT;
    entity_data JSONB; -- Use JSONB for entity_data as well
BEGIN
    -- Loop through each entity related to the customer
    FOR entity_name IN
        SELECT DISTINCT entity 
        FROM relationships 
        WHERE relationships.customer_id = get_customer_relationships.customer_id
    LOOP
        -- Execute dynamic SQL to fetch data for the current entity, ensure empty array if no data
        EXECUTE format('SELECT COALESCE(jsonb_agg(row_to_json(t)), ''[]''::JSONB) FROM (SELECT * FROM %I WHERE id IN (SELECT entity_id FROM relationships WHERE relationships.customer_id = $1 AND entity = $2)) t', entity_name)
        INTO entity_data
        USING get_customer_relationships.customer_id, entity_name;

        -- Add the entity data to the result JSONB object
        result := result || jsonb_build_object(entity_name, entity_data);
    END LOOP;

    -- Convert JSONB to JSON for the return type
    RETURN result::JSON;
END;
$$ LANGUAGE plpgsql;