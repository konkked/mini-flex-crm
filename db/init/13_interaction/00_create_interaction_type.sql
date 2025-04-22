DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = 'interaction_type') THEN
        CREATE TYPE interaction_type AS ENUM ('email', 'call', 'in_person_meeting', 'virtual_meeting', 'social_media', 'text_message', 'video_call', 'webinar', 'networking_event');
    END IF;
END $$;
