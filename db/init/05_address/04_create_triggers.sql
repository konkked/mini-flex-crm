
-- Trigger to call the function before insert
CREATE TRIGGER before_insert_entity_contact
BEFORE INSERT ON entity_contact
FOR EACH ROW
EXECUTE FUNCTION entityAddress_set_significance_ordinal();


-- Trigger to call the function before update
CREATE TRIGGER before_update_entity_contact
BEFORE UPDATE ON entity_contact
FOR EACH ROW
EXECUTE FUNCTION entityAddress_adjust_significance_ordinal();