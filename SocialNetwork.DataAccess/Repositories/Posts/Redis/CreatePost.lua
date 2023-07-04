#!lua name=postsrepolib

local function User(userId) 
    return 'User:' .. tostring(userId) 
end

local function UserPosts(userId) 
    return User(userId) .. ':Posts'
end

local function UserPostsList(userId) 
    return UserPosts(userId) .. ':List'
end

local function Posts() 
    return 'Posts'
end

local function PostsCount() 
    return 'Posts:Count'
end

local function GetPostId(keys, argv)
    return redis.call('INCR', PostsCount())
end

local function SaveUserPostLink(postId, userId)
    redis.call('HSET', Posts(), postId, userId)
end

local function PushToPostsList(postId, userId) 
    redis.call('LPUSH', UserPostsList(userId), postId)
end

local function PopFromPostsList(postId, userId) 
    redis.call('LREM', UserPostsList(userId), 0, postId)
end

local function DeleteUserPostLink(postId) 
    redis.call('HDEL', Posts(), postId)
end

local function GetAuthor(postId) 
    return redis.call('HGET', Posts(), postId)
end

local function UpdatePost(keys, argv)
    local serializedPost = keys[1] 
    local userId = keys[2] 
    local postId = keys[3]
    
    redis.call('HSET', UserPosts(userId), postId, serializedPost)
end

local function CreatePost(keys, argv)
    local serializedPost = keys[1]
    local userId = keys[2]
    local postId = keys[3]

    redis.call('HSET', UserPosts(userId), postId, serializedPost)
    SaveUserPostLink(postId, userId)
    PushToPostsList(postId, userId)
end

local function DeletePost(keys, argv)
    local userId = keys[1]
    local postId = keys[2]
    
    DeleteUserPostLink(postId)
    PopFromPostsList(postId, userId)
    redis.call('HDEL', UserPosts(userId), postId)
end

local function GetPostById(keys, argv)
    local postId = keys[1]
    
    local userId = GetAuthor(postId)

    if userId == nil then
        return nil
    end
    
    return redis.call('HGET', UserPosts(userId), postId)
end

local function GetUserPosts(keys, argv) 
    local userId = keys[1]
    local start = keys[2]
    local stop = keys[3]
    
    return redis.call('LRANGE', UserPostsList(userId), start, stop)
end

redis.register_function('get_user_posts', GetUserPosts)
redis.register_function('get_post_id', GetPostId)
redis.register_function('create_post', CreatePost)
redis.register_function('update_post', UpdatePost)
redis.register_function('delete_post', DeletePost)
redis.register_function('get_post_by_id', GetPostById)
