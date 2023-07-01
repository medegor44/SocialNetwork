#!lua name=postsrepolib

local function User(userId) 
    return 'User:' .. tostring(userId) 
end

local function UserPosts(userId) 
    return User(userId) .. ':Posts'
end

local function UserPostsCount(userId) 
    return UserPosts(userId) .. ':Count'
end

local function GetPostId(keys, argv)
    local userId = keys[1]
    
    return redis.call('INCR', UserPostsCount(userId))
end

-- create, update
-- EVAL "SavePost(ARGV[1], ARGV[2], ARGV[3])" 0 {...} 1 2"
local function SavePost(keys, argv)
    local serializedPost = keys[1] 
    local userId = keys[2] 
    local postId = keys[3]
    
    redis.call('HSET', UserPosts(userId), tostring(postId), serializedPost)
end

-- delete
local function DeletePost(keys, argv)
    local userId = keys[1]
    local postId = keys[2]
    
    redis.call('HDEL', UserPosts(userId), tostring(postId))
end

-- get
local function GetPostById(keys, argv)
    local userId = keys[1]
    local postId = keys[2]
    
    return redis.call('HGET', UserPosts(userId), tostring(postId))
end

redis.register_function('get_post_id', GetPostId)
redis.register_function('save_post', SavePost)
redis.register_function('delete_post', DeletePost)
redis.register_function('get_post_by_id', GetPostById)
