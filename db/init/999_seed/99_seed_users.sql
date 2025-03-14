INSERT INTO "app_user" (username, name, email, password_hash, salt, role, enabled, tenant_id) VALUES
('user1', 'User One', 'user1@example.com', 'o2mAD9hQCuAeFClTb09w7t2efPKfNTxAgQrpRyNURAI=', 'g8eY0NC2wVYWA2y4chGP7Q==', 'standard', true, 1),
('user1', 'Admin One', 'admin1@example.com', 'o2mAD9hQCuAeFClTb09w7t2efPKfNTxAgQrpRyNURAI=', 'g8eY0NC2wVYWA2y4chGP7Q==', 'admin', true, 1),
('user2', 'User Two', 'user2@example.com', 'o2mAD9hQCuAeFClTb09w7t2efPKfNTxAgQrpRyNURAI=', 'g8eY0NC2wVYWA2y4chGP7Q==', 'standard', true, 2),
('user1', 'Admin Two', 'admin2@example.com', 'o2mAD9hQCuAeFClTb09w7t2efPKfNTxAgQrpRyNURAI=', 'g8eY0NC2wVYWA2y4chGP7Q==', 'admin', true, 2),
('user3', 'User Three', 'user3@example.com', 'o2mAD9hQCuAeFClTb09w7t2efPKfNTxAgQrpRyNURAI=', 'g8eY0NC2wVYWA2y4chGP7Q==', 'standard', true, 3),
('user1', 'Admin Three', 'admin3@example.com', 'o2mAD9hQCuAeFClTb09w7t2efPKfNTxAgQrpRyNURAI=', 'g8eY0NC2wVYWA2y4chGP7Q==', 'admin', true, 3)
ON CONFLICT DO NOTHING;