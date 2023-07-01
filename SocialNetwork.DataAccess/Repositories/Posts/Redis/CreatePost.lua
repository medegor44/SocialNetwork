#!lua name=postsrepolib

local function User(userId) 
    return 'User:' .. tostring(userId) 
end

local function UserPosts(userId) 
    return User(userId) .. ':Posts'
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
    redis.call('HSET', Posts(), tostring(postId), tostring(userId))
end

local function DeleteUserPostLink(postId) 
    redis.call('HDEL', Posts(), tostring(postId))
end

local function GetAuthor(postId) 
    return redis.call('HGET', Posts(), tostring(postId))
end

local function SavePost(keys, argv)
    local serializedPost = keys[1] 
    local userId = keys[2] 
    local postId = keys[3]
    
    redis.call('HSET', UserPosts(userId), tostring(postId), serializedPost)
    SaveUserPostLink(postId, userId)
end

local function DeletePost(keys, argv)
    local userId = keys[1]
    local postId = keys[2]
    
    DeleteUserPostLink(postId)
    redis.call('HDEL', UserPosts(userId), tostring(postId))
end

local function GetPostById(keys, argv)
    local postId = keys[1]
    
    local userId = GetAuthor(postId)

    if userId == nil then
        return nil
    end
    
    return redis.call('HGET', UserPosts(userId), tostring(postId))
end

redis.register_function('get_post_id', GetPostId)
redis.register_function('save_post', SavePost)
redis.register_function('delete_post', DeletePost)
redis.register_function('get_post_by_id', GetPostById)
