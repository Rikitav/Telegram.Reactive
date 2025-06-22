using System.Collections;
using System.Collections.ObjectModel;
using System.Reflection;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.Payments;
using Telegram.Reactive.Core.Collections;
using Telegram.Reactive.Core.Components.Filters;
using Telegram.Reactive.Core.Components.Handlers;

#pragma warning disable IDE0130
namespace Telegram.Reactive;
#pragma warning restore IDE0130

/// <summary>
/// Provides extension methods for working with collections.
/// </summary>
public static partial class ColletionsExtensions
{
    /// <summary>
    /// Creates a <see cref="ReadOnlyDictionary{TKey, TValue}"/> from an <see cref="IEnumerable{TValue}"/>
    /// according to a specified key selector function.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="source"></param>
    /// <param name="keySelector"></param>
    /// <returns></returns>
    public static ReadOnlyDictionary<TKey, TValue> ToReadOnlyDictionary<TKey, TValue>(this IEnumerable<TValue> source, Func<TValue, TKey> keySelector) where TKey : notnull
    {
        Dictionary<TKey, TValue> dictionary = source.ToDictionary(keySelector);
        return new ReadOnlyDictionary<TKey, TValue>(dictionary);
    }

    /// <summary>
    /// Enumerates objects in a <paramref name="source"/> and executes an <paramref name="action"/> on each one
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="source"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static IEnumerable<TValue> ForEach<TValue>(this IEnumerable<TValue> source, Action<TValue> action)
    {
        foreach (TValue value in source)
            action.Invoke(value);

        return source;
    }

    /// <summary>
    /// Creates a new <see cref="IEnumerable{T}"/> with the elements of the <paramref name="source"/> that were successfully cast to the <typeparamref name="TResult"/>
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static IEnumerable<TResult> WhereCast<TResult>(this IEnumerable source)
    {
        foreach (object value in source)
        {
            if (value is TResult result)
                yield return result;
        }
    }

    /// <summary>
    /// Sets the value of a key in a dictionary, or if the key does not exist, adds it
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="source"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public static void Set<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, TValue value)
    {
        if (source.ContainsKey(key))
            source[key] = value;
        else
            source.Add(key, value);
    }

    /// <summary>
    /// Sets the value of a key in a dictionary, or if the key does not exist, adds its default value.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="source"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="defaultValue"></param>
    public static void Set<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, TValue value, TValue defaultValue)
    {
        if (source.ContainsKey(key))
            source[key] = value;
        else
            source.Add(key, defaultValue);
    }

    /// <summary>
    /// Return the random object from <paramref name="source"/>
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static TSource Random<TSource>(this IEnumerable<TSource> source)
        => source.Random(new Random());

    /// <summary>
    /// Return the random object from <paramref name="source"/>
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source"></param>
    /// <param name="random"></param>
    /// <returns></returns>
    public static TSource Random<TSource>(this IEnumerable<TSource> source, Random random)
        => source.ElementAt(random.Next(0, source.Count() - 1));
}

/// <summary>
/// Provides extension methods for reflection and type inspection.
/// </summary>
public static partial class ReflectionExtensions
{
    private static readonly BindingFlags BindAll = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;

    /// <summary>
    /// Checks if <paramref name="type"/> is a <see cref="IFilter{T}"/>
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsFilterType(this Type type)
        => type.IsAssignableToGenericType(typeof(IFilter<>));

    /// <summary>
    /// Checks if <paramref name="type"/> is a descendant of <see cref="UpdateHandlerBase"/> class
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsHandlerAbstract(this Type type)
        => type.IsAbstract && typeof(UpdateHandlerBase).IsAssignableFrom(type);

    /// <summary>
    /// Checks if <paramref name="type"/> is an implementation of <see cref="UpdateHandlerBase"/> class or its descendants
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsHandlerRealization(this Type type)
        => !type.IsAbstract && type != typeof(UpdateHandlerBase) && typeof(UpdateHandlerBase).IsAssignableFrom(type);

    /// <summary>
    /// Checks if <paramref name="type"/> has a parameterless constructor
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool HasParameterlessCtor(this Type type)
        => type.GetConstructors().Any(ctor => ctor.GetParameters().Length == 0);

    /// <summary>
    /// Invokes a "<paramref name="methodName"/>" method of an object
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="methodName"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public static object? InvokeMethod(this object obj, string methodName, params object[]? args)
        => obj.GetType().GetMethod(methodName, BindAll).InvokeMethod(obj, args);

    /// <summary>
    /// Invokes a method of <paramref name="methodInfo"/>
    /// </summary>
    /// <param name="methodInfo"></param>
    /// <param name="obj"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public static object? InvokeMethod(this MethodInfo methodInfo, object obj, params object[]? args)
        => methodInfo.Invoke(obj, args);

    /// <summary>
    /// Invokes a static method of <paramref name="methodInfo"/>
    /// </summary>
    /// <param name="methodInfo"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public static object? InvokeStaticMethod(this MethodInfo methodInfo, params object[]? parameters)
        => methodInfo.Invoke(null, parameters);

    /// <summary>
    /// Invokes a static "<paramref name="methodName"/>" method of an object
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="methodName"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public static T? InvokeStaticMethod<T>(this object obj, string methodName, params object[]? parameters)
        => (T?)obj.GetType().GetMethod(methodName, BindAll).InvokeStaticMethod(parameters);

    /// <summary>
    /// Invokes a generic method of <paramref name="methodInfo"/> with generic types in <paramref name="genericParameters"/>
    /// </summary>
    /// <param name="methodInfo"></param>
    /// <param name="obj"></param>
    /// <param name="genericParameters"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public static object InvokeGenericMethod(this MethodInfo methodInfo, object obj, Type[] genericParameters, params object[]? parameters)
        => methodInfo.MakeGenericMethod(genericParameters).Invoke(obj, parameters);

    /// <summary>
    /// Invokes a generic <paramref name="methodName"/> method with generic types in <paramref name="genericParameters"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="methodName"></param>
    /// <param name="genericParameters"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public static T? InvokeGenericMethod<T>(this object obj, string methodName, Type[] genericParameters, params object[]? parameters)
        => (T?)obj.GetType().GetMethod(methodName).InvokeGenericMethod(obj, genericParameters, parameters);

    /// <summary>
    /// Checks is <paramref name="obj"/> has public properties
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool HasPublicProperties(this object obj)
        => obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).Any();

    /// <summary>
    /// Determines whether an instance of a specified type can be assigned to an instance of the current type
    /// </summary>
    /// <param name="givenType"></param>
    /// <param name="genericType"></param>
    /// <returns></returns>
    public static bool IsAssignableToGenericType(this Type givenType, Type genericType)
    {
        if (givenType.GetInterfaces().Any(inter => inter.IsGenericType && inter.GetGenericTypeDefinition() == genericType))
            return true;

        if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
            return true;

        if (givenType.BaseType == null)
            return false;

        return givenType.BaseType.IsAssignableToGenericType(genericType);
    }
}

/// <summary>
/// Provides extension methods for string manipulation.
/// </summary>
public static partial class StringExtensions
{
    /// <summary>
    /// Slices a <paramref name="source"/> string into a array of substrings of fixed <paramref name="length"/>
    /// </summary>
    /// <param name="source"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public static IEnumerable<string> SliceBy(this string source, int length)
    {
        for (int start = 0; start < source.Length; start += length + 1)
        {
            int tillEnd = source.Length - start;
            int toSlice = tillEnd < length + 1 ? tillEnd : length + 1;

            ReadOnlySpan<char> chunk = source.AsSpan().Slice(start, toSlice);
            yield return chunk.ToString();
        }
    }
}

/// <summary>
/// Provides extension methods for working with Telegram Update objects.
/// </summary>
public static partial class UpdateExtensions
{
    /// <summary>
    /// Selects from Update an object from which you can get the sender's ID
    /// </summary>
    /// <param name="update"></param>
    /// <returns>Sender's ID</returns>
    public static long? GetSenderId(this Update update) => update switch
    {
        { Message.From: { } from } => from.Id,
        { Message.SenderChat: { } chat } => chat.Id,
        { EditedMessage.From: { } from } => from.Id,
        { EditedMessage.SenderChat: { } chat } => chat.Id,
        { ChannelPost.From: { } from } => from.Id,
        { ChannelPost.SenderChat: { } chat } => chat.Id,
        { EditedChannelPost.From: { } from } => from.Id,
        { EditedChannelPost.SenderChat: { } chat } => chat.Id,
        { CallbackQuery.From: { } from } => from.Id,
        { InlineQuery.From: { } from } => from.Id,
        { PollAnswer.User: { } user } => user.Id,
        { PreCheckoutQuery.From: { } from } => from.Id,
        { ShippingQuery.From: { } from } => from.Id,
        { ChosenInlineResult.From: { } from } => from.Id,
        { ChatJoinRequest.From: { } from } => from.Id,
        { ChatMember.From: { } from } => from.Id,
        { MyChatMember.From: { } from } => from.Id,
        _ => null
    };

    /// <summary>
    /// Selects from Update an object from which you can get the chat's ID
    /// </summary>
    /// <param name="update"></param>
    /// <returns>Sender's ID</returns>
    public static long? GetChatId(this Update update) => update switch
    {
        { Message.Chat: { } chat } => chat.Id,
        { Message.SenderChat: { } chat } => chat.Id,
        { EditedMessage.Chat: { } chat } => chat.Id,
        { EditedMessage.SenderChat: { } chat } => chat.Id,
        { ChannelPost.Chat: { } chat } => chat.Id,
        { ChannelPost.SenderChat: { } chat } => chat.Id,
        { EditedChannelPost.Chat: { } chat } => chat.Id,
        { EditedChannelPost.SenderChat: { } chat } => chat.Id,
        { CallbackQuery.Message.Chat: { } chat } => chat.Id,
        { ChatJoinRequest.Chat: { } chat } => chat.Id,
        { ChatMember.Chat: { } chat } => chat.Id,
        { MyChatMember.Chat: { } chat } => chat.Id,
        _ => null
    };

    /// <summary>
    /// Selects from <see cref="Update"/> an object that contains information about the update
    /// </summary>
    /// <param name="update"></param>
    /// <returns></returns>
    public static object GetActualUpdateObject(this Update update) => update switch
    {
        { Message: { } message } => message,
        { EditedMessage: { } editedMessage } => editedMessage,
        { ChannelPost: { } channelPost } => channelPost,
        { EditedChannelPost: { } editedChannelPost } => editedChannelPost,
        { BusinessConnection: { } businessConnection } => businessConnection,
        { BusinessMessage: { } businessMessage } => businessMessage,
        { EditedBusinessMessage: { } editedBusinessMessage } => editedBusinessMessage,
        { DeletedBusinessMessages: { } deletedBusinessMessages } => deletedBusinessMessages,
        { MessageReaction: { } messageReaction } => messageReaction,
        { MessageReactionCount: { } messageReactionCount } => messageReactionCount,
        { InlineQuery: { } inlineQuery } => inlineQuery,
        { ChosenInlineResult: { } chosenInlineResult } => chosenInlineResult,
        { CallbackQuery: { } callbackQuery } => callbackQuery,
        { ShippingQuery: { } shippingQuery } => shippingQuery,
        { PreCheckoutQuery: { } preCheckoutQuery } => preCheckoutQuery,
        { PurchasedPaidMedia: { } purchasedPaidMedia } => purchasedPaidMedia,
        { Poll: { } poll } => poll,
        { PollAnswer: { } pollAnswer } => pollAnswer,
        { MyChatMember: { } myChatMember } => myChatMember,
        { ChatMember: { } chatMember } => chatMember,
        { ChatJoinRequest: { } chatJoinRequest } => chatJoinRequest,
        { ChatBoost: { } chatBoost } => chatBoost,
        { RemovedChatBoost: { } removedChatBoost } => removedChatBoost,
        _ => update
    };

    /// <summary>
    /// Selects from <see cref="Update"/> an <typeparamref name="T"/> that contains information about the update
    /// </summary>
    /// <param name="update"></param>
    /// <returns></returns>
    public static T GetActualUpdateObject<T>(this Update update)
    {
        object actualUpdate = update.GetActualUpdateObject() ?? throw new Exception();
        if (actualUpdate is not T actualCasted)
            throw new Exception();

        return actualCasted;
    }
}

/// <summary>
/// Provides extension methods for working with UpdateType enums.
/// </summary>
public static partial class UpdateTypeExtensions
{
    /// <summary>
    /// <see cref="UpdateType"/>'s that contain a message
    /// </summary>
    public static readonly UpdateType[] MessageTypes =
    [
        UpdateType.Message,
            UpdateType.EditedMessage,
            UpdateType.BusinessMessage,
            UpdateType.EditedBusinessMessage,
            UpdateType.ChannelPost,
            UpdateType.EditedChannelPost
    ];

    /// <summary>
    /// Checks if <typeparamref name="T"/> matches one of the <see cref="UpdateType"/>'s give on <paramref name="allowedTypes"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="allowedTypes"></param>
    /// <returns></returns>
    public static bool IsUpdateObjectAllowed<T>(this UpdateType[] allowedTypes) where T : class
    {
        return allowedTypes.Any(t => t.IsValidUpdateObject<T>());
    }

    /// <summary>
    /// Checks if <typeparamref name="TUpdate"/> matches the given <see cref="UpdateType"/>
    /// </summary>
    /// <typeparam name="TUpdate"></typeparam>
    /// <param name="updateType"></param>
    /// <returns></returns>
    public static bool IsValidUpdateObject<TUpdate>(this UpdateType updateType) where TUpdate : class
    {
        if (typeof(TUpdate) == typeof(Update))
            return true;

        return typeof(TUpdate).Equals(updateType.ReflectUpdateObject());
    }

    /// <summary>
    /// Returns an update object corresponding to the <see cref="UpdateType"/>.
    /// </summary>
    /// <param name="updateType"></param>
    /// <returns></returns>
    public static Type? ReflectUpdateObject(this UpdateType updateType)
    {
        return updateType switch
        {
            UpdateType.Message or UpdateType.EditedMessage or UpdateType.BusinessMessage or UpdateType.EditedBusinessMessage or UpdateType.ChannelPost or UpdateType.EditedChannelPost => typeof(Message),
            UpdateType.MyChatMember => typeof(ChatMemberUpdated),
            UpdateType.ChatMember => typeof(ChatMemberUpdated),
            UpdateType.InlineQuery => typeof(InlineQuery),
            UpdateType.ChosenInlineResult => typeof(ChosenInlineResult),
            UpdateType.CallbackQuery => typeof(CallbackQuery),
            UpdateType.ShippingQuery => typeof(ShippingQuery),
            UpdateType.PreCheckoutQuery => typeof(PreCheckoutQuery),
            UpdateType.Poll => typeof(Poll),
            UpdateType.PollAnswer => typeof(PollAnswer),
            UpdateType.ChatJoinRequest => typeof(ChatJoinRequest),
            UpdateType.MessageReaction => typeof(MessageReactionUpdated),
            UpdateType.MessageReactionCount => typeof(MessageReactionCountUpdated),
            UpdateType.ChatBoost => typeof(ChatBoostUpdated),
            UpdateType.RemovedChatBoost => typeof(ChatBoostRemoved),
            UpdateType.BusinessConnection => typeof(BusinessConnection),
            UpdateType.DeletedBusinessMessages => typeof(BusinessMessagesDeleted),
            UpdateType.PurchasedPaidMedia => typeof(PaidMediaPurchased),
            _ or UpdateType.Unknown => typeof(Update)
        };
    }
}
