local composer = require "composer"
local bf = require "basic_features"
local physics = require "physics"

local scene = composer.newScene()


-- -----------------------------------------------------------------------------------
-- Code outside of the scene event functions below will only be executed ONCE unless
-- the scene is removed entirely (not recycled) via "composer.removeScene()"
-- -----------------------------------------------------------------------------------


local score = 0
local level_of_bullet = 1
local num_of_enemy = 0
local phase = 0

function game_pause( )

    local sceneOption =
    {
        effect = "fade",
        tiem = 500,
        isModal = true,
        params =
        {
            gamescore = score,
        },
        isModal = true
    }

    composer.showOverlay( "pause", sceneOption )
end

function game_over( )

    local sceneOption =
    {
        effect = "fade",
        time = 500,
        params =
        {
            gamescore = score,
            defeatedEnemy = num_of_enemy,
            bulletLevel = level_of_bullet
        },
    }
    composer.removeScene( "game", true )
    composer.gotoScene( "score", scoreOption )
end

local character_image
function create_display()
    local character_movement_left, character_movement_right = 0, 0
    local character_flow_weight, delay_time = 5, 500

    function character_flow_movement()
        if phase == 0 then
            local character_y = character_image.y
            transition.to( character_image, { time = delay_time, y = character_y + character_flow_weight, transition = easing.inOutSine } )
            transition.to( character_image, { time = delay_time, y = character_y, transition = easing.inOutSine, delay = delay_time })
        end
    end

    function character_movement()
        local max_speed, per_speed = 5, 0.05
        local character_movement_weight

        if phase == 0 then
            if is_right then
                character_movement_right = character_movement_right + per_speed

                if character_movement_right >= max_speed then
                    character_movement_right = max_speed
                end
            else
                character_movement_right = 0
            end

            if is_left then
                character_movement_left = character_movement_left - per_speed

                if character_movement_left <= -max_speed then
                    character_movement_left = -max_speed
                end
            else
                character_movement_left = 0
            end

            character_movement_weight = character_movement_left + character_movement_right

            character_image.x = character_image.x + character_movement_weight
            -- print(character_movement_weight)

            if character_image.x <= character_image.contentWidth*0.5 then
                character_image.x = character_image.contentWidth*0.5
            elseif character_image.x >= bf.W - character_image.contentWidth*0.5 then
                character_image.x = bf.W - character_image.contentWidth*0.5
            end
        end
    end

    physics.start()
    physics.setGravity( 0, 0 )
    physics.setDrawMode( "hybrid" )

    character_image = display.newImage( "resource/char.png", bf.W * 0.5, bf.H * 0.85 )
    physics.addBody( character_image, (require "character").physicsData(2.0):get("char"))
    character_image:scale(2,2)

    timer.performWithDelay( delay_time * 2, character_flow_movement, -1 )
    Runtime:addEventListener( "enterFrame", character_movement )

    wall = display.newRect( 0, -20, bf.W*1.5, 5 )
    wall.anchorX, wall.anchorY = 0, 0
    wall.name = "wall"
    physics.addBody( wall, "static", {} )
end

function character_shoot()
    local bullet
    local bullet_speed = { 330, 500, 750 }
    local bullet_attack = { 1, 3, 5 }
    local bullet_rink

    function enable_bullet()
        is_shoot = true
    end

    function bullet_collision( self, event )
        local phase = event.phase

        if phase == "began" then
            if self.name == "bullet" and event.other.name == "wall" then
                self:removeSelf()
            elseif self.name == "bullet" and event.other.name == "enemy" then
                self:removeSelf()

                event.other.hp = event.ohter.hp - self.attatck

                if event.other.hp <= 0 then
                    event.other:removeSelf()
                end
            end
        end
    end

    timer.performWithDelay( bullet_speed[level_of_bullet], enable_bullet )

    bullet_rink = "bullet".. ( level_of_bullet == 3 and "C" or level_of_bullet == 2 and "B" or "A" )
    print( "bullet rink : " .. bullet_rink )

    bullet = display.newImage( "resource/" .. bullet_rink, character_image.x, character_image.y )
    physics.addBody( bullet, (require "bullet"):physicsData(2.0):get( bullet_rink ) )
    bullet:scale(2,2)

    bullet.isBullet = true
    bullet.name = "bullet"
    bullet.attack = bullet_speed[level_of_bullet]
    bullet.speed = bullet_speed[level_of_bullet]
    bullet.collision = bullet_collision

    bullet:setLinearVelocity( 1000, 0 )
end

local is_shoot = true
function key_event( e )
    print( e.keyName .. " : " .. e.phase )
    if e.keyName == "left" then
        if e.phase == "down" then
            is_left = true
        elseif e.phase == "up" then
            is_left = false
        end
    end

    if e.keyName == "right" then
        if e.phase == "down" then
            is_right = true
        elseif e.phase == "up" then
            is_right = false
        end
    end

    if e.keyName == "leftControl" then
        if e.phase == "down" then
            if is_shoot == true then
                is_shoot = false
                character_shoot()
            end
        elseif e.phase == "up" then
        end
    end
end

-- -----------------------------------------------------------------------------------
-- Scene event functions
-- -----------------------------------------------------------------------------------


-- create()
function scene:create( e )

    local sceneGroup = self.view

    create_display()
    Runtime:addEventListener( "key", key_event )

end


-- show()
function scene:show( e )

    local sceneGroup = self.view
    local phase = e.phase

    if phase == "will" then
        -- Code here runs when the scene is still off screen ( but is about to come on screen )

    elseif phase == "did" then
        -- Code here runs when the scene is entirely on screen

    end
end


-- hide()
function scene:hide( e )

    local sceneGroup = self.view
    local phase = e.phase

    if phase == "will" then
        -- Code here runs when the scene is on screen (but is about to go off screen)

    elseif phase == "did" then
        -- Code here runs immediately after the scene goes entirely off screen

    end
end


-- destroy()
function scene:destroy( e )

    local sceneGroup = self.view
    -- Code here runs prior to the removal of scene's view

end


-- -----------------------------------------------------------------------------------
-- Scene event function listeners
-- -----------------------------------------------------------------------------------
scene:addEventListener( "create", scene )
scene:addEventListener( "show", scene )
scene:addEventListener( "hide", scene )
scene:addEventListener( "destroy", scene )
-- -----------------------------------------------------------------------------------

return scene
