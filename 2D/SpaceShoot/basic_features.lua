local basic_features = {}

basic_features.W = display.contentWidth
basic_features.H = display.contentHeight

function basic_features.hexToPercent(hex)
    local r = tonumber( hex:sub(1, 2), 16 ) / 255
    local g = tonumber( hex:sub(3, 4), 16 ) / 255
    local b = tonumber( hex:sub(5, 6), 16 ) / 255
    local a = 255 / 255

    if #hex == 8 then
        a = tonumber( hex:sub(7, 8), 16) / 255
    end

    return r, g, b, a
end

basic_features.font = native.newFont("정9체.ttf")
basic_features.font_size = 75
basic_features.font_size_b = 170

return basic_features
