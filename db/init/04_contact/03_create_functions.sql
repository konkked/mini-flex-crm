-- Function to set the significance_ordinal for insert
CREATE OR REPLACE FUNCTION entityContact_set_significance_ordinal()
RETURNS TRIGGER AS $$
BEGIN
    NEW.significance_ordinal := COALESCE(
        (SELECT MAX(significance_ordinal) + 1 FROM entity_contact WHERE entity_name = NEW.entity_name AND entity_id = NEW.entity_id),
        1
    );
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Function to adjust significance_ordinal for update
CREATE OR REPLACE FUNCTION entityContact_adjust_significance_ordinal()
RETURNS TRIGGER AS $$
BEGIN
    -- Adjust significance_ordinal for other related items
    UPDATE entity_contact
    SET significance_ordinal = significance_ordinal + 1
    WHERE entity_name = NEW.entity_name
      AND entity_id = NEW.entity_id
      AND significance_ordinal >= NEW.significance_ordinal
      AND id <> NEW.id;

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;