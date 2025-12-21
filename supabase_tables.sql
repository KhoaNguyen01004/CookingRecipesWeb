-- Create users table
CREATE TABLE users (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    first_name TEXT NOT NULL,
    last_name TEXT NOT NULL,
    email TEXT UNIQUE NOT NULL
);

-- Create favorites table
CREATE TABLE favorites (
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    recipe_id TEXT NOT NULL,
    PRIMARY KEY (user_id, recipe_id)
);

-- Optional: Enable Row Level Security (RLS) if needed
-- ALTER TABLE users ENABLE ROW LEVEL SECURITY;
ALTER TABLE favorites DISABLE ROW LEVEL SECURITY;
