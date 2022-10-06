using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FastMember
{
    internal static class ReflectionExpressionGenerator
    {
        public static Func<object, object> GenerateFieldGetter(FieldInfo field)
        {
            ParameterExpression instance = Expression.Parameter(typeof(object), "instance");

            Expression<Func<object, object>> lambda = Expression.Lambda<Func<object, object>>(
                Expression.Convert(
                    Expression.Field(
                        CreateInstanceExpression(field, instance)
                        , field)
                    , typeof(object))
                , instance);

            return lambda.Compile();
        }

        public static Action<object, object> GenerateFieldSetter(FieldInfo field)
        {
            ParameterExpression instance = Expression.Parameter(typeof(object), "instance");
            ParameterExpression value = Expression.Parameter(typeof(object), "value");

            MemberExpression left = Expression.Field(CreateInstanceExpression(field, instance), field);
            UnaryExpression right = Expression.Convert(value, field.FieldType);

            Expression<Action<object, object>> lambda = Expression.Lambda<Action<object, object>>(
                Expression.Assign(left, right)
                , instance, value);

            return lambda.Compile();
        }

        public static Func<object, object> GeneratePropertyGetter(PropertyInfo property)
        {
            ParameterExpression instance = Expression.Parameter(typeof(object), "instance");

            Expression<Func<object, object>> lambda = Expression.Lambda<Func<object, object>>(
                Expression.Convert(
                    Expression.Property(
                        CreateInstanceExpression(property, instance)
                        , property)
                    , typeof(object))
                , instance);

            return lambda.Compile();
        }

        public static Action<object, object> GeneratePropertySetter(PropertyInfo property)
        {
            ParameterExpression instance = Expression.Parameter(typeof(object), "instance");
            ParameterExpression value = Expression.Parameter(typeof(object), "value");

            MemberExpression left = Expression.Property(CreateInstanceExpression(property, instance), property);
            UnaryExpression right = Expression.Convert(value, property.PropertyType);

            Expression<Action<object, object>> lambda = Expression.Lambda<Action<object, object>>(
                Expression.Assign(left, right)
                , instance, value);

            return lambda.Compile();
        }

        public static Func<object[], object> GenerateConstructorInvoker(ConstructorInfo constructor)
        {
            ParameterExpression args = Expression.Parameter(typeof(object[]), "args");

            Expression<Func<object[], object>> lambda = Expression.Lambda<Func<object[], object>>(
                Expression.New(constructor, CreateParameterConverterExpression(constructor, args))
                , args);

            return lambda.Compile();
        }

        public static Func<object, object[], object> GenerateMethodInvoker(MethodInfo method)
        {
            ParameterExpression instance = Expression.Parameter(typeof(object), "instance");
            ParameterExpression args = Expression.Parameter(typeof(object[]), "args");

            MethodCallExpression methodCall = Expression.Call(
                CreateInstanceExpression(method, instance)
                , method
                , CreateParameterConverterExpression(method, args));

            Expression<Func<object, object[], object>> lambda = Expression.Lambda<Func<object, object[], object>>(
                method.ReturnType == typeof(void) ?
                    Expression.Block(typeof(object), methodCall, Expression.Constant(null)) :
                    (Expression)Expression.Convert(methodCall, typeof(object))
                , instance, args);

            return lambda.Compile();
        }

        private static Expression CreateInstanceExpression(FieldInfo field, ParameterExpression instance)
            => field.IsStatic ? null : Expression.Convert(instance, field.DeclaringType);

        private static Expression CreateInstanceExpression(PropertyInfo property, ParameterExpression instance)
            => CreateInstanceExpression(property.GetAccessors(true)[0], instance);

        private static Expression CreateInstanceExpression(MethodInfo method, ParameterExpression instance)
            => method.IsStatic ? null : Expression.Convert(instance, method.DeclaringType);

        private static IEnumerable<Expression> CreateParameterConverterExpression(MethodBase method, ParameterExpression args)
        {
            return method.GetParameters().Select((parameter, i) =>
            {
                return Expression.Convert(
                    Expression.ArrayIndex(args, Expression.Constant(i))
                    , parameter.ParameterType);
            });
        }
    }
}
