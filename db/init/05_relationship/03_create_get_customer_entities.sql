CREATE OR REPLACE FUNCTION get_customer_relationships(customer_id INT) RETURNS JSON AS $$
DECLARE
    result JSON := '{}'::JSON; -- Initialize result as an empty JSON object
    entity_name TEXT;
    entity_data JSON;
BEGIN
    -- Loop through each entity related to the customer
    FOR entity_name IN
        SELECT DISTINCT entity FROM relation WHERE customer_id = customer_id
    LOOP
        -- Execute dynamic SQL to fetch data for the current entity
        EXECUTE format('SELECT json_agg(row_to_json(t)) FROM (SELECT * FROM %I WHERE id IN (SELECT entity_id FROM relationships WHERE customer_id = $1 AND entity = $2)) t', entity_name)
        INTO entity_data
        USING customer_id, entity_name;

        -- If entity_data is not null, add it to the result JSON object
        IF entity_data IS NOT NULL THEN
            result := result || json_build_object(entity_name, entity_data);
        END IF;
    END LOOP;

    RETURN result;
END;
$$ LANGUAGE plpgsql;